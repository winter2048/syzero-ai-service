using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dynamitey.DynamicObjects;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR;
using SyZero.Cache;
using SyZero.OpenAI.Core.OpenAI;
using SyZero.OpenAI.IApplication.Chat;
using SyZero.OpenAI.IApplication.Chat.Dto;
using SyZero.Runtime.Security;
using SyZero.Runtime.Session;
using SyZero.Util;

namespace SyZero.OpenAI.Web.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public static Dictionary<long, string> ActiveConnections = new Dictionary<long, string>();
        private readonly OpenAIService _openAIService;
        private readonly ICache _cache;

        public ChatHub(OpenAIService openAIService, ICache cache)
        {
            _openAIService = openAIService;
            _cache = cache;
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
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", GetSessions(userId));

                var res = _openAIService.ChatCompletionAsync(new Core.OpenAI.Dto.ChatRequest()
                {
                    Model = messageDto.Model,
                    Messages = chatSession.Messages.Where(p=> !p.Content.Contains("data:image/png;base64")).Select(p => new Core.OpenAI.Dto.Message { Role = p.Role.ToString().ToLower(), Content = p.Content }).ToList()
                });

                chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.Assistant, ""));
                await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);

                await foreach (var item in res)
                {
                    chatSession.Messages.LastOrDefault(p=>p.Role == MessageRoleEnum.Assistant).Content += item.Choices[0]?.Delta?.Content;
                    Console.Write(item.Choices[0]?.Delta?.Content);
                    await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", GetSessions(userId));
                }

                // 判断是否生成图片
                Match imageMatch = Regex.Match(chatSession.Messages.LastOrDefault(p => p.Role == MessageRoleEnum.Assistant).Content, @"(?<=\$\[image\]\()(.*?)(?=\))");
                if (imageMatch.Success)
                {
                    string imageContent = imageMatch.Groups[1].Value;
                    string imageBase64 = "data:image/png;base64," + (await _openAIService.ImageGeneration(new Core.OpenAI.Dto.ImageRequest(imageContent))).Data[0].Base64;
                    chatSession.Messages.LastOrDefault(p => p.Role == MessageRoleEnum.Assistant).Content = @$"![{imageContent}]({imageBase64})";
                    await _cache.SetAsync($"ChatSession:{userId}:{messageDto.SessionId}", chatSession.Messages);
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", GetSessions(userId));
                }
            }
        }

        private List<ChatSessionDto> GetSessions(long userId)
        {
            List<ChatSessionDto> list = new List<ChatSessionDto>();
            var keys = _cache.GetKeys($"ChatSession:{userId}:*");
            foreach (var sessionId in keys.Select(p => p.Split(":").Last()).ToArray())
            {
                var messages = _cache.Get<List<ChatMessageDto>>($"ChatSession:{userId}:{sessionId}");
                list.Add(new ChatSessionDto()
                {
                    Id = sessionId,
                    Messages = messages
                }
                );
            }
            list = list.OrderByDescending(p => p.Messages?.LastOrDefault()?.Date ?? DateTime.Now).ToList();
            return list;
        }
    }
}
