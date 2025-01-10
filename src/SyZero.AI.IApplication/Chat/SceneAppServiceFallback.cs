using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SyZero.Cache;
using SyZero.Client;
using SyZero.Logger;
using SyZero.AI.IApplication.Chat.Dto;
using SyZero.Runtime.Security;
using SyZero.Serialization;

namespace SyZero.AI.IApplication.Chat
{
    public class SceneAppServiceFallback : ISceneAppService, IFallback
    {
        private readonly ILogger _logger;

        public SceneAppServiceFallback(ILogger logger)
        {
            _logger = logger;
        }

        public Task<SceneDto> CreateScene(SceneDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DelScene(string sceneId)
        {
            throw new NotImplementedException();
        }

        public Task<SceneDto> GetScene(string sceneId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SceneDto>> MyScene()
        {
            throw new NotImplementedException();
        }

        public Task<SceneDto> PutScene(string sceneId, SceneDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
