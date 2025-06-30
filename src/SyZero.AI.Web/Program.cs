using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using OpenAI;
using System;
using System.ClientModel;
using System.Threading;
using SyZero;
using SyZero.AI.Core;
using SyZero.AI.Repository;
using SyZero.AI.Web.Filter;
using SyZero.AI.Web.Hub;
using SyZero.DynamicWebApi;
using SyZero.Web.Common;

namespace SyZero.AI.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var builder = WebApplication.CreateBuilder(args);

            //使用SyZero
            builder.AddSyZero();

            //builder.Configuration.AddNacos(cancellationTokenSource.Token); //Nacos动态配置
            builder.Configuration.AddConsul(cancellationTokenSource.Token);

            builder.WebHost.UseUrls($"{AppConfig.ServerOptions.Protocol}://*:{AppConfig.ServerOptions.Port}");

            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            }).AddSyZeroLog4Net();

            builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            builder.Services.AddControllers().AddMvcOptions(options =>
            {
                options.Filters.Add(new AppExceptionFilter());
                options.Filters.Add(new AppResultFilter());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new LongToStrConverter());
            });

            //动态WebApi
            builder.Services.AddDynamicWebApi(new DynamicWebApiOptions()
            {
                DefaultApiPrefix = "/api",
                DefaultAreaName = AppConfig.ServerOptions.Name
            });
            //Swagger
            builder.Services.AddSwagger();
            //使用OpenTelemetry遥测
            builder.Services.AddSyZeroOpenTelemetry();
            //使用AutoMapper
            builder.Services.AddSyZeroAutoMapper();
            //使用SqlSugar仓储
            builder.Services.AddSyZeroSqlSugar<DbContext>();
            //注入控制器
            builder.Services.AddSyZeroController();
            //注入Redis
            builder.Services.AddSyZeroRedis();
            //注入公共层
            builder.Services.AddSyZeroCommon();
            //注入Consul
            builder.Services.AddConsul();
            //注入Feign
            builder.Services.AddSyZeroFeign();

            builder.Services.AddSignalR();

            var openAIClient = new OpenAIClient(new ApiKeyCredential(AppConfig.GetSection("OpenAIToken")), new OpenAIClientOptions() { Endpoint = new Uri(AppConfig.GetSection("OpenAIUrl") ?? "https://api.openai.com") });

            builder.Services.AddKeyedChatClient(AIProvider.Ollama, new OllamaApiClient(new Uri(AppConfig.GetSection("OllamaAIUrl")), AIChatModel.Ollama_Qwen25_7B.ToModel()));
            builder.Services.AddKeyedChatClient(AIProvider.OpenAI, openAIClient.AsChatClient(AIChatModel.OpenAI_GPT4OMini.ToModel()));

            builder.Services.AddKeyedEmbeddingGenerator(AIProvider.Ollama, new OllamaApiClient(AppConfig.GetSection("OllamaAIUrl"), AIEmbeddingModel.Ollama_Nomic_Embed_Text.ToModel()));
            builder.Services.AddKeyedEmbeddingGenerator(AIProvider.OpenAI, openAIClient.AsEmbeddingGenerator(AIEmbeddingModel.OpenAI_Text_Embedding3_Large.ToModel()));

            builder.Services.AddSingleton<OpenAIService>();
            builder.Services.AddSingleton<VectorStoreService>();

            var app = builder.Build();

            app.UseSyZero();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
            {
                builder.AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
            app.UseRouting();
            app.UseStaticFiles();
            app.UseSyAuthMiddleware((sySeesion) => "Token:" + sySeesion.UserId);
            app.MapControllers();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SyZero.AI.Web API V1");
                c.RoutePrefix = "api/swagger";

            });
            app.UseConsul();

            app.MapHub<ChatHub>("/chathub");

            app.InitTables();

            app.Run();
        }
    }
}



