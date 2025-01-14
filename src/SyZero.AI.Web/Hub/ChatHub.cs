using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dynamitey.DynamicObjects;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR;
using SyZero.Cache;
using SyZero.AI.IApplication.Chat;
using SyZero.AI.IApplication.Chat.Dto;
using SyZero.Runtime.Security;
using SyZero.Runtime.Session;
using SyZero.Util;
using SyZero.AI.Core;
using Microsoft.Extensions.AI;
using SyZero.ObjectMapper;
using Renci.SshNet.Security;
using Org.BouncyCastle.Crypto.Tls;

namespace SyZero.AI.Web.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public static Dictionary<long, HashSet<string>> ActiveConnections = new Dictionary<long, HashSet<string>>();
        private readonly OpenAIService _openAIService;
        private readonly IObjectMapper _objectMapper;
        private readonly ICache _cache;

        public ChatHub(OpenAIService openAIService, ICache cache, IObjectMapper objectMapper)
        {
            _openAIService = openAIService;
            _cache = cache;
            _objectMapper = objectMapper;
        }

        public override Task OnConnectedAsync()
        {
            if (Context.GetHttpContext().Request.Query.TryGetValue("accessToken", out var accessToken))
            {
                // 从accessToken获取登录信息
                var sy = SyZeroUtil.GetService<ISySession>().Parse(accessToken);

                if (sy?.UserId.HasValue == true)
                {
                    if (ActiveConnections.Any(p => p.Key == sy.UserId.Value))
                    {
                        //Clients.Client(ActiveConnections[sy.UserId.Value]).SendAsync("Disconnect");
                        ActiveConnections[sy.UserId.Value].Add(Context.ConnectionId);
                    }
                    else
                    {
                        ActiveConnections.Add(sy.UserId.Value, new HashSet<string>() { Context.ConnectionId });
                    }
                }
                
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var item in ActiveConnections.Where(p => p.Value.Contains(Context.ConnectionId)))
            {
                item.Value.Remove(Context.ConnectionId);
                if (item.Value.Count == 0)
                {
                    ActiveConnections.Remove(item.Key);
                }
            };
           
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDto messageDto)
        {
            if (ActiveConnections.Any(p => p.Value.Contains(Context.ConnectionId)))
            {
                var activeConnection = ActiveConnections.FirstOrDefault(p => p.Value.Contains(Context.ConnectionId));
                var userId = activeConnection.Key;
                var connectionIds = activeConnection.Value;

                var messages = _cache.Get<List<ChatMessageDto>>($"ChatSession:{userId}:{messageDto.SessionId}");
                var chatSession = new ChatSessionDto()
                {
                    Id = messageDto.SessionId,
                    Messages = messages
                };
                chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.User, messageDto.Message));
                await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);

                await ClientBatchSend(connectionIds, "ReceiveMessage", GetSessions(userId, messageDto.SessionId));

                var res = _openAIService.ChatCompletionAsync(_objectMapper.Map<List<ChatMessage>>(chatSession.Messages), messageDto.Model);

                chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.Assistant, ""));
                await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);

                await foreach (var item in res)
                {
                    chatSession.Messages.LastOrDefault(p => p.Role == MessageRoleEnum.Assistant).Content += item.Text;
                    Console.Write(item.Text);
                    await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);
                    await ClientBatchSend(connectionIds, "ReceiveMessage", GetSessions(userId, messageDto.SessionId));
                }
            }
        }

        private List<ChatSessionDto> GetSessions(long userId)
        {
            List<ChatSessionDto> list = new List<ChatSessionDto>();
            var keys = _cache.GetKeys($"ChatSession:{userId}:*");
            foreach (var sessionId in keys.Select(p => p.Split(":").Last()).ToArray())
            {
                list.Add(GetSessions(userId, sessionId));
            }
            list = list.OrderByDescending(p => p.Messages?.LastOrDefault()?.Date ?? DateTime.Now).ToList();
            return list;
        }

        private ChatSessionDto GetSessions(long userId, string sessionId)
        {
            var messages = _cache.Get<List<ChatMessageDto>>($"ChatSession:{userId}:{sessionId}");
            return new ChatSessionDto()
            {
                Id = sessionId,
                Messages = messages
            };
        }

        private async Task ClientBatchSend(IEnumerable<string> connectionIds, string method, object obj)
        {
            var sendTask = connectionIds.Select(connectionId => Clients.Client(connectionId).SendAsync(method, obj));
            await Task.WhenAll(sendTask);
        }
    }
}
