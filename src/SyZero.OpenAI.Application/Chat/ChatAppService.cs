using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SyZero;
using SyZero.Application.Service;
using SyZero.Cache;
using SyZero.Logger;
using SyZero.Runtime.Security;
using SyZero.Runtime.Session;
using SyZero.Serialization;
using SyZero.Web.Common;
using SyZero.OpenAI.IApplication.Chat;
using SyZero.OpenAI.Repository;
using System.Linq;
using Org.BouncyCastle.Bcpg;
using System.Net;
using System.Collections.Generic;
using SyZero.OpenAI.IApplication.Chat.Dto;
using SyZero.Client;
using System.Data;
using SqlSugar.Extensions;
using SyZero.OpenAI.Core.OpenAI;
using SqlSugar;
using System.Text.RegularExpressions;

namespace SyZero.OpenAI.Application.Chat
{
    public class ChatAppService : ApplicationService, IChatAppService
    {
        private readonly ICache _cache;
        private readonly ISyEncode _syEncode;
        private readonly IToken _token;
        private readonly IJsonSerialize _jsonSerialize;
        private readonly ILogger _logger;
        private readonly OpenAIService _openAIService;

        public ChatAppService(
           ICache cache,
           ISyEncode syEncode,
           IToken token,
           IJsonSerialize jsonSerialize,
           ILogger logger,
           OpenAIService openAIService)
        {
            _cache = cache;
            _syEncode = syEncode;
            _token = token;
            _jsonSerialize = jsonSerialize;
            _logger = logger;
            _openAIService = openAIService;
        }

        public async Task<Dictionary<string, string>> GetModels()
        {
            CheckPermission("");
            return await _openAIService.GetModels();
        }

        public async Task<string> CreateSession()
        {
            CheckPermission("");
            var keys = _cache.GetKeys($"ChatSession:{SySession.UserId}:*");
            var sessionId = keys.Select(p => p.Split(":").Last()).FirstOrDefault(sessionId => _cache.Get<List<ChatMessageDto>>($"ChatSession:{SySession.UserId}:{sessionId}").Count == 0);
            if (sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString();
                var messages = new List<ChatMessageDto>();
                await _cache.SetAsync($"ChatSession:{SySession.UserId}:{sessionId}", messages);
            }
            return sessionId;
        }

        public async Task DelSession(string sessionId)
        {
            CheckPermission("");
            if (!_cache.Exist($"ChatSession:{SySession.UserId}:{sessionId}"))
            {
                throw new SyMessageException("会话不存在！");
            }

            _cache.Remove($"ChatSession:{SySession.UserId}:{sessionId}");
        }

        public async Task<ChatSessionDto> GetSession(string sessionId)
        {
            CheckPermission("");
            if (!_cache.Exist($"ChatSession:{SySession.UserId}:{sessionId}"))
            {
                throw new SyMessageException("会话不存在！");
            }

            var messages = _cache.Get<List<ChatMessageDto>>($"ChatSession:{SySession.UserId}:{sessionId}");
            return new ChatSessionDto()
            {
                Id = sessionId,
                Messages = messages
            };
        }

        public async Task<List<ChatSessionDto>> MySession()
        {
            CheckPermission("");
            List<ChatSessionDto> list = new List<ChatSessionDto>();
            var keys = _cache.GetKeys($"ChatSession:{SySession.UserId}:*");
            foreach (var sessionId in keys.Select(p => p.Split(":").Last()).ToArray())
            {
                var messages = _cache.Get<List<ChatMessageDto>>($"ChatSession:{SySession.UserId}:{sessionId}");
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

        public async Task<bool> PutSession(string sessionId, List<ChatMessageDto> messages)
        {
            CheckPermission("");
            if (!_cache.Exist($"ChatSession:{SySession.UserId}:{sessionId}"))
            {
                throw new SyMessageException("会话不存在！");
            }
            if (messages == null)
            {
                throw new SyMessageException("消息不能为空！");
            }
            await _cache.SetAsync($"ChatSession:{SySession.UserId}:{sessionId}", messages);
            return true;
        }

        public async Task<string> SendMessage(SendMessageDto messageDto)
        {
            CheckPermission("");
            var chatSession = await GetSession(messageDto.SessionId);
            chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.User, messageDto.Message));

            var res = await _openAIService.ChatCompletion(new Core.OpenAI.Dto.ChatRequest()
            {
                Model = messageDto.Model,
                Messages = chatSession.Messages.Where(p => !p.Content.Contains("data:image/png;base64")).Select(p => new Core.OpenAI.Dto.Message { Role = p.Role.ToString().ToLower(), Content = p.Content }).ToList()
            });

            string content = res.Choices[0]?.Message?.Content;
            Console.Write(content);

            // 判断是否生成图片
            Match imageMatch = Regex.Match(content, @"(?<=\$\[image\]\()(.*?)(?=\))");
            if (imageMatch.Success)
            {
                string imageContent = imageMatch.Groups[1].Value;
                string imageBase64 = "data:image/png;base64," + (await _openAIService.ImageGeneration(new Core.OpenAI.Dto.ImageRequest(imageContent))).Data[0].Base64;
                content = @$"![{imageContent}]({imageBase64})";
            }

            chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.Assistant, content));
            await _cache.SetAsync($"ChatSession:{SySession.UserId}:{messageDto.SessionId}", chatSession.Messages);
            // return res.Choices[0].Message.Content;
            return content;
        }
    }
}



