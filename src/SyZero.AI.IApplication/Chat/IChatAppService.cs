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
        [Get("Models")]
        Task<Dictionary<string, string>> GetModels();

        /// <summary>
        /// 创建会话
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Post("Session")]
        Task<string> CreateSession();

        /// <summary>
        /// 更新会话
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Put("Session/{sessionId}")]
        Task<bool> PutSession(string sessionId, List<ChatMessageDto> messages);

        /// <summary>
        /// 删除会话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [Delete("Session/{sessionId}")]
        Task DelSession(string sessionId);

        /// <summary>
        /// 查询会话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [Get("Session/{sessionId}")]
        Task<ChatSessionDto> GetSession(string sessionId);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        [Post("SendMessage")]
        Task<string> SendMessage(SendMessageDto messageDto);

        /// <summary>
        /// 获取我的所以会话
        /// </summary>
        /// <returns></returns>
        [Get("Sessions")]
        Task<List<ChatSessionDto>> MySession();
    }
}



