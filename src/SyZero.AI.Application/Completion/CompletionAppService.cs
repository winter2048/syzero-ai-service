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
using System.Linq;
using Org.BouncyCastle.Bcpg;
using System.Net;
using System.Collections.Generic;
using SyZero.AI.IApplication.Chat.Dto;
using SyZero.Client;
using System.Data;
using SqlSugar.Extensions;
using SqlSugar;
using SyZero.AI.IApplication.Completion;
using SyZero.AI.IApplication.Completion.Dto;
using SyZero.AI.Core;
using SyZero.FileStore.IApplication.Container;
using SyZero.Util;

namespace SyZero.AI.Application.Completion
{
    public class CompletionAppService : ApplicationService, ICompletionAppService
    {
        private readonly ICache _cache;
        private readonly ISyEncode _syEncode;
        private readonly IToken _token;
        private readonly IJsonSerialize _jsonSerialize;
        private readonly ILogger _logger;
        private readonly OpenAIService _openAIService;
        private readonly VectorStoreService _vectorStoreService;

        public CompletionAppService(
           ICache cache,
           ISyEncode syEncode,
           IToken token,
           IJsonSerialize jsonSerialize,
           ILogger logger,
           OpenAIService openAIService,
           VectorStoreService vectorStoreService)
        {
            _cache = cache;
            _syEncode = syEncode;
            _token = token;
            _jsonSerialize = jsonSerialize;
            _logger = logger;
            _openAIService = openAIService;
            _vectorStoreService = vectorStoreService;
        }

        public async Task<string> Send(CompletionDto completionDto)
        {
            await Console.Out.WriteLineAsync("ssss");
            var sss = SyZeroUtil.GetService<IContainerAppService>();
            string msg = "";
            try
            {
                var test = await sss.GetContainerList();
                msg = _jsonSerialize.ObjectToJSON(test);
            }
            catch (Exception ex)
            {
                throw;
            }
            

            //CheckPermission("");
            //var res = await _openAIService.Completion(new Core.OpenAI.Dto.CompletionRequest()
            //{
            //    Model = "text-davinci-003",
            //    Prompt = completionDto.Message
            //});

            //var a = await _openAIService.GenerateEmbeddingVectorAsync(completionDto.Message, AIEmbeddingModel.Ollama_Nomic_Embed_Text.ToDescription());

            //await _vectorStoreService.Test(111111111, a);

            //await _vectorStoreService.SearchAsync(a);

            return msg;
        }

        public Task<string> Send2(string user)
        {
            throw new NotImplementedException();
        }
    }
}