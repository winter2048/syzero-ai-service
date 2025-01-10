using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyZero.Application.Service;
using SyZero.Cache;
using SyZero.Logger;
using SyZero.AI.Core.Chat;
using SyZero.AI.IApplication.Chat;
using SyZero.AI.IApplication.Chat.Dto;
using SyZero.AI.Repository.Repository;
using SyZero.Runtime.Security;
using SyZero.Serialization;
using SyZero.Web.Common;
using SyZero.AI.Core;

namespace SyZero.AI.Application.Chat
{
    public class SceneAppService : ApplicationService, ISceneAppService
    {
        private readonly ICache _cache;
        private readonly ISyEncode _syEncode;
        private readonly IToken _token;
        private readonly IJsonSerialize _jsonSerialize;
        private readonly ILogger _logger;
        private readonly OpenAIService _openAIService;
        private readonly ISceneRepository _sceneRepository;

        public SceneAppService(
           ICache cache,
           ISyEncode syEncode,
           IToken token,
           IJsonSerialize jsonSerialize,
           ILogger logger,
           OpenAIService openAIService,
           ISceneRepository sceneRepository)
        {
            _cache = cache;
            _syEncode = syEncode;
            _token = token;
            _jsonSerialize = jsonSerialize;
            _logger = logger;
            _openAIService = openAIService;
            _sceneRepository = sceneRepository;
        }

        public async Task<SceneDto> CreateScene(SceneDto dto)
        {
            CheckPermission("");
            var entity = ObjectMapper.Map<Scene>(dto);
            entity.CreateTime = DateTime.Now;
            entity.CreateUser = SySession.UserId.Value;
            await _sceneRepository.AddAsync(entity);
            return ObjectMapper.Map<SceneDto>(entity);
        }

        public async Task<bool> DelScene(string sceneId)
        {
            CheckPermission("");
            var entity = await _sceneRepository.GetModelAsync(sceneId.ToLong());
            if (entity.CreateUser == SySession.UserId)
            {
                return await _sceneRepository.DeleteAsync(sceneId.ToLong()) > 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<SceneDto> GetScene(string sceneId)
        {
            CheckPermission("");
            var scene = await _sceneRepository.GetModelAsync(p => p.Id == sceneId.ToLong());
            return ObjectMapper.Map<SceneDto>(scene);
        }

        public async Task<List<SceneDto>> MyScene()
        {
            CheckPermission("");
            var scene = await _sceneRepository.GetListAsync(p => p.CreateUser == SySession.UserId || p.IsDefault);
            return ObjectMapper.Map<List<SceneDto>>(scene.OrderByDescending(p => p.IsDefault).ToList());
        }

        public async Task<SceneDto> PutScene(string sceneId, SceneDto dto)
        {
            CheckPermission("");
            var entity = await _sceneRepository.GetModelAsync(sceneId.ToLong());
            ObjectMapper.Map(dto, entity);
            if (await _sceneRepository.UpdateAsync(entity) > 0)
            {
                return ObjectMapper.Map<SceneDto>(entity);
            }
            else
            {
                throw new SyMessageException("修改失败！");
            }
        }
    }
}
