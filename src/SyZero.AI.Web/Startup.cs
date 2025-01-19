using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using SyZero;
using SyZero.AspNetCore;
using SyZero.AutoMapper;
using SyZero.Consul;
using SyZero.DynamicWebApi;
using SyZero.Feign;
using SyZero.Log4Net;
using SyZero.Redis;
using SyZero.Web.Common;
using SyZero.AI.Repository;
using SyZero.AI.Web.Filter;
using System.Net;
using SyZero.AI.Web.Hub;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using SyZero.AI.Core;
using Microsoft.AspNetCore.Http;
using OllamaSharp;

namespace SyZero.AI.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AppConfig.Configuration = configuration;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
         
            services.AddControllers().AddMvcOptions(options =>
            {
                options.Filters.Add(new AppExceptionFilter());
                options.Filters.Add(new AppResultFilter());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new LongToStrConverter());
            });

            services.AddSyZero();

            //动态WebApi
            services.AddDynamicWebApi(new DynamicWebApiOptions()
            {
                DefaultApiPrefix = "/api",
                DefaultAreaName = AppConfig.ServerOptions.Name
            });

            //Swagger
            services.AddSwagger();
            //使用AutoMapper
            services.AddSyZeroAutoMapper();
            //使用SqlSugar仓储
            services.AddSyZeroSqlSugar<DbContext>();
            //注入控制器
            services.AddSyZeroController();
            //注入公共层
            services.AddSyZeroCommon();
            //注入Log4Net
            services.AddSyZeroLog4Net();
            //注入Redis
            services.AddSyZeroRedis();
            //注入Consul
            services.AddConsul();
            //注入Feign
            services.AddSyZeroFeign();

            services.AddSignalR();

            var openAIClient = new OpenAIClient(new ApiKeyCredential(AppConfig.GetSection("OpenAIToken")), new OpenAIClientOptions() { Endpoint = new Uri(AppConfig.GetSection("OpenAIUrl") ?? "https://api.openai.com") } );

            services.AddKeyedChatClient(AIProvider.Ollama, new OllamaApiClient(new Uri(AppConfig.GetSection("OllamaAIUrl")), AIChatModel.Ollama_Qwen25_7B.ToModel()));
            services.AddKeyedChatClient(AIProvider.OpenAI, openAIClient.AsChatClient(AIChatModel.OpenAI_GPT4OMini.ToModel()));

            services.AddKeyedEmbeddingGenerator(AIProvider.Ollama, new OllamaApiClient(AppConfig.GetSection("OllamaAIUrl"), AIEmbeddingModel.Ollama_Nomic_Embed_Text.ToModel()));
            services.AddKeyedEmbeddingGenerator(AIProvider.OpenAI, openAIClient.AsEmbeddingGenerator(AIEmbeddingModel.OpenAI_Text_Embedding3_Large.ToModel()));

            services.AddSingleton<OpenAIService>();
            services.AddSingleton<VectorStoreService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
            app.UseSyAuthMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SyZero.Authorization.Web API V1");
                c.RoutePrefix = "api/swagger";

            });
            app.UseConsul();
            app.UseSyZero();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chathub");
            });
            app.InitTables();
        }
    }
}



