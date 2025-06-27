using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SyZero.Cache;
using SyZero.Client;
using SyZero.AI.IApplication.Chat.Dto;
using SyZero.Runtime.Security;
using SyZero.Serialization;
using Microsoft.Extensions.Logging;

namespace SyZero.AI.IApplication.Chat
{
    public class ChatAppServiceFallback : IChatAppService, IFallback
    {
        private readonly ILogger<ChatAppServiceFallback> _logger;

        public ChatAppServiceFallback(ILogger<ChatAppServiceFallback> logger)
        {
            _logger = logger;
        }

        public Task<string> CreateSession()
        {
            _logger.LogError("Fallback => ChatAppService:Chat");
            return null;
        }

        public Task DelSession(string sessionId)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetModels()
        {
            throw new NotImplementedException();
        }

        public Task<ChatSessionDto> GetSession(string sessionId)
        {
            _logger.LogError("Fallback => ChatAppService:Chat");
            return null;
        }

        public Task<List<ChatSessionDto>> MySession()
        {
            _logger.LogError("Fallback => ChatAppService:Chat");
            return null;
        }

        public Task<bool> PutSession(string sessionId, List<ChatMessageDto> messages)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendMessage(SendMessageDto messageDto)
        {
            _logger.LogError("Fallback => ChatAppService:Chat");
            return null;
        }
    }
}
