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
using SyZero.AI.IApplication.Chat;
using SyZero.AI.Repository;
using System.Linq;
using Org.BouncyCastle.Bcpg;
using System.Net;
using System.Collections.Generic;
using SyZero.AI.IApplication.Chat.Dto;
using SyZero.Client;
using System.Data;
using SqlSugar.Extensions;
using SqlSugar;
using System.Text.RegularExpressions;
using SyZero.AI.Core;
using SyZero.AI.Core.Chat;
using Microsoft.Extensions.AI;

namespace SyZero.AI.Application.Chat
{
    public class ChatAppService : ApplicationService, IChatAppService
    {
        private readonly ICache _cache;
        private readonly ISyEncode _syEncode;
        private readonly IToken _token;
        private readonly IJsonSerialize _jsonSerialize;
        private readonly ILogger<ChatAppService> _logger;
        private readonly OpenAIService _openAIService;

        public ChatAppService(
           ICache cache,
           ISyEncode syEncode,
           IToken token,
           IJsonSerialize jsonSerialize,
           ILogger<ChatAppService> logger,
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

        /// <inheritdoc/>
        public async Task<string> SendMessage(SendMessageDto messageDto)
        {
            CheckPermission("");
            var chatSession = await GetSession(messageDto.SessionId);
            chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.User, messageDto.Message));
            _logger.Info(string.Format("Chat: [{0}][{1}] {2}", messageDto.Model, MessageRoleEnum.User.ToString(), messageDto.Message));

            var res = await _openAIService.ChatCompletion(ObjectMapper.Map<List<ChatMessage>>(chatSession.Messages), messageDto.Model);
            string content = res.Message.Text;
            _logger.Info(string.Format("Chat: [{0}][{1}] {2}", messageDto.Model, MessageRoleEnum.Assistant.ToString(), content));

            chatSession.Messages.Add(new ChatMessageDto(MessageRoleEnum.Assistant, content));
            await _cache.SetAsync($"ChatSession:{SySession.UserId}:{messageDto.SessionId}", chatSession.Messages);
            return content;
        }
    }
}



