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

namespace SyZero.AI.Web.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public static Dictionary<long, string> ActiveConnections = new Dictionary<long, string>();
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
                var sy = SyZeroUtil.GetService<ISySession>().Parse(accessToken);

                // 从accessToken获取登录信息
                if (ActiveConnections.Any(p=>p.Key == sy.UserId.Value))
                {
                    Clients.Client(ActiveConnections[sy.UserId.Value]).SendAsync("Disconnect");
                    ActiveConnections[sy.UserId.Value] = Context.ConnectionId;
                }
                else
                {
                    ActiveConnections.Add(sy.UserId.Value, Context.ConnectionId);
                }
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var Key in ActiveConnections.Where(p => p.Value == Context.ConnectionId).Select(p => p.Key).ToList())
            {
                ActiveConnections.Remove(Key);
            };
           
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDto messageDto)
        {
            if (ActiveConnections.Any(p => p.Value == Context.ConnectionId))
            {
                var userId = ActiveConnections.FirstOrDefault(p => p.Value == Context.ConnectionId).Key;

                var messages = _cache.Get<List<ChatMessageDto>>($"ChatSession:{userId}:{messageDto.SessionId}");
                var chatSession = new ChatSessionDto()
                {
                    Id = messageDto.SessionId,
                    Messages = messages
                };
                chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.User, messageDto.Message));
                await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", GetSessions(userId, messageDto.SessionId));

                var res = _openAIService.ChatCompletionAsync(_objectMapper.Map<List<ChatMessage>>(chatSession.Messages), messageDto.Model);

                chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.Assistant, ""));
                await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);

                await foreach (var item in res)
                {
                    chatSession.Messages.LastOrDefault(p=>p.Role == MessageRoleEnum.Assistant).Content += item.Text;
                    Console.Write(item.Text);
                    await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", GetSessions(userId, messageDto.SessionId));
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
    }
}
