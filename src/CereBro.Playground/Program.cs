using System;
using System.Threading.Tasks;
using CereBro.OpenAI;
using Microsoft.Extensions.Hosting;

namespace CereBro.Playground
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.UseOpenAI(Environment.GetEnvironmentVariable("OPEN_AI_API_KEY"), "gpt-4o-mini");
            
            IHost cereBro = builder.BuildCereBro(new CereBroConfig{ ServersFilePath = "./servers.json" });

            await cereBro.RunAsync();
        }
    }
}