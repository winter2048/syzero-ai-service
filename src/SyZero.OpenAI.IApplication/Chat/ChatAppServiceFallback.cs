﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SyZero.Cache;
using SyZero.Client;
using SyZero.Logger;
using SyZero.OpenAI.IApplication.Chat.Dto;
using SyZero.Runtime.Security;
using SyZero.Serialization;

namespace SyZero.OpenAI.IApplication.Chat
{
    public class ChatAppServiceFallback : IChatAppService, IFallback
    {
        private readonly ILogger _logger;

        public ChatAppServiceFallback(ILogger logger)
        {
            _logger = logger;
        }

        public Task<string> CreateSession()
        {
            _logger.Error("Fallback => ChatAppService:Chat");
            return null;
        }

        public Task DelSession(string sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<ChatSessionDto> GetSession(string sessionId)
        {
            _logger.Error("Fallback => ChatAppService:Chat");
            return null;
        }

        public Task<string[]> MySession()
        {
            _logger.Error("Fallback => ChatAppService:Chat");
            return null;
        }

        public Task<string> SendMessage(SendMessageDto messageDto)
        {
            _logger.Error("Fallback => ChatAppService:Chat");
            return null;
        }
    }
}
