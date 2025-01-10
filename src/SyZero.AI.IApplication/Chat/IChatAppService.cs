using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using SyZero.Application.Attributes;
using SyZero.Application.Routing;
using SyZero.AI.IApplication.Chat.Dto;

namespace SyZero.AI.IApplication.Chat
{
    public interface IChatAppService : IApplicationServiceBase
    {
        /// <summary>
        /// 获取模型列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ApiMethod(HttpMethod.GET, "/api/SyZero.AI/Chat/Models")]
        Task<Dictionary<string, string>> GetModels();

        /// <summary>
        /// 创建会话
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ApiMethod(HttpMethod.POST, "/api/SyZero.AI/Chat/Session")]
        Task<string> CreateSession();

        /// <summary>
        /// 更新会话
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ApiMethod(HttpMethod.PUT, "/api/SyZero.AI/Chat/Session/{sessionId}")]
        Task<bool> PutSession(string sessionId, List<ChatMessageDto> messages);

        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [ApiMethod(HttpMethod.DELETE, "/api/SyZero.AI/Chat/Session/{sessionId}")]
        Task DelSession(string sessionId);

        /// <summary>
        /// 查询会话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [ApiMethod(HttpMethod.GET, "/api/SyZero.AI/Chat/Session/{sessionId}")]
        Task<ChatSessionDto> GetSession(string sessionId);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        [ApiMethod(HttpMethod.POST)]
        Task<string> SendMessage(SendMessageDto messageDto);

        /// <summary>
        /// 获取我的所以会话
        /// </summary>
        /// <returns></returns>
        [ApiMethod(HttpMethod.GET, "/api/SyZero.AI/Chat/Sessions")]
        Task<List<ChatSessionDto>> MySession();
    }
}



