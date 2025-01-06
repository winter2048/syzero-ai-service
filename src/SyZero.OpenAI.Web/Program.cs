using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using SyZero;

namespace SyZero.OpenAI.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, builder) =>
               {
                   //builder.AddNacos(cancellationTokenSource.Token); //Nacos动态配置
                   builder.AddConsul(cancellationTokenSource.Token);  //Consul动态配置
               })
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseUrls($"{AppConfig.ServerOptions.Protocol}://*:{AppConfig.ServerOptions.Port}").UseStartup<Startup>();
               }).Build().Run();
        }
    }
}



