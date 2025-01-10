using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyZero.Web.Common;
using Newtonsoft.Json;
using RestSharp;
using SyZero.Util;
using System.IO;
using Org.BouncyCastle.Asn1.Crmf;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using SqlSugar;
using OllamaSharp;
using OpenAI.Models;
using System.ClientModel;
using OllamaSharp.Models;
using Google.Protobuf.WellKnownTypes;

namespace SyZero.AI.Core
{
    public class OpenAIService
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenAIService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IChatClient GetChatClient(AIProvider provider) => _serviceProvider.GetRequiredKeyedService<IChatClient>(provider);

        public IChatClient GetChatClient(string model) => model.IsOllama() ? GetChatClient(AIProvider.Ollama) : GetChatClient(AIProvider.OpenAI);

        public IEmbeddingGenerator<string, Embedding<float>> GetEmbeddingGenerator(AIProvider provider) => _serviceProvider.GetRequiredKeyedService<IEmbeddingGenerator<string, Embedding<float>>>(provider);

        public IEmbeddingGenerator<string, Embedding<float>> GetEmbeddingGenerator(string model) => model.IsOllama() ? GetEmbeddingGenerator(AIProvider.Ollama) : GetEmbeddingGenerator(AIProvider.OpenAI);

        public async Task<Dictionary<string, string>> GetModels()
        {
            var ollamaApiClient = GetChatClient(AIProvider.Ollama).GetService<OllamaApiClient>();
            var openAIClient = GetChatClient(AIProvider.OpenAI).GetService<OpenAIClient>();

            var ollamaModelsTask = ollamaApiClient.ListLocalModelsAsync().ContinueWith(p =>
            {
                if (!p.IsCompletedSuccessfully)
                {
                    return new Dictionary<string, string>();
                }
                return p.Result.ToDictionary(value => value.Name, value => $"{AIProvider.Ollama}|{value.Name}");
            });
            var openAIModelsTask = openAIClient.GetOpenAIModelClient().GetModelsAsync().ContinueWith(p =>
            {
                if (!p.IsCompletedSuccessfully)
                {
                    return new Dictionary<string, string>();
                }
                return p.Result.Value.ToDictionary(value => value.Id, value => $"{AIProvider.OpenAI}|{value.Id}");
            });

            var completedTasks = await Task.WhenAll(ollamaModelsTask, openAIModelsTask);

            return completedTasks[0].Concat(completedTasks[1]).GroupBy(kvp => kvp.Key).ToDictionary(g => g.Key, g => g.Last().Value);
        }

        public async Task<ChatCompletion> ChatCompletion(List<ChatMessage> chatMessages, string aIChatModel)
        {
            var cetChatClient = GetChatClient(aIChatModel);

            return await cetChatClient.CompleteAsync(chatMessages, new ChatOptions()
            {
                ModelId = aIChatModel.ToModel(),
            });
        }

        public IAsyncEnumerable<StreamingChatCompletionUpdate> ChatCompletionAsync(List<ChatMessage> chatMessages, string aIChatModel)
        {
            var cetChatClient = GetChatClient(aIChatModel);

            return cetChatClient.CompleteStreamingAsync(chatMessages, new ChatOptions()
            {
                ModelId = aIChatModel.ToModel(),
            });
        }

        public async Task<ReadOnlyMemory<float>> GenerateEmbeddingVectorAsync(string message, string aIEmbeddingModel)
        {
            var embeddingGenerator = GetEmbeddingGenerator(aIEmbeddingModel);

            return await embeddingGenerator.GenerateEmbeddingVectorAsync(message, new EmbeddingGenerationOptions()
            {
                ModelId = aIEmbeddingModel.ToModel()
            });
        }
    }
}
