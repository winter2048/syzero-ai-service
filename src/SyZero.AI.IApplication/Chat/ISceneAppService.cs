using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Application.Routing;
using SyZero.AI.IApplication.Chat.Dto;

namespace SyZero.AI.IApplication.Chat
{
    public interface ISceneAppService : IApplicationServiceBase
    {
        /// <summary>
        /// 创建场景
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Post]
        Task<SceneDto> CreateScene(SceneDto dto);

        /// <summary>
        /// 修改场景
        /// </summary>
        /// <param name="sceneId"></param>
        /// <returns></returns>
        [Put("{sceneId}")]
        Task<SceneDto> PutScene(string sceneId, SceneDto dto);

        /// <summary>
        /// 获取场景
        /// </summary>
        /// <param name="sceneId"></param>
        /// <returns></returns>
        [Get("{sceneId}")]
        Task<SceneDto> GetScene(string sceneId);

        /// <summary>
        /// 获取我的所以场景
        /// </summary>
        /// <param name="sceneId"></param>
        /// <returns></returns>
        [Get]
        Task<List<SceneDto>> MyScene();

        /// <summary>
        /// 删除场景
        /// </summary>
        /// <param name="sceneId"></param>
        /// <returns></returns>
        [Delete("{sceneId}")]
        Task<bool>DelScene(string sceneId);
    }
}
