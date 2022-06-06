using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PhotoMaster
{
    internal class Program
    {
        public async static Task Main(string[] args)
        {
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                IHost host = Program.CreateHostBuilder(args).Build();
                await host.Services.GetService<PhotoHelper>().Run(tokenSource.Token);
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddGooglePhotos();
                    services.AddSingleton<PhotoHelper>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole();
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddUserSecrets<Program>();
                });
        }
    }
}
