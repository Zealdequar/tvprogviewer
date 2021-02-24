using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TVProgViewer.Services.Configuration;

namespace TVProgViewer.WebUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .ConfigureAppConfiguration(configuration => configuration.AddJsonFile(TvProgConfigurationDefaults.AppSettingsFilePath, true, true))
                    .UseStartup<Startup>())
                .Build()
                .RunAsync();
        }
    }
}