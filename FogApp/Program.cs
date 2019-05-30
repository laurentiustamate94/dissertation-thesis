using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FogApp
{
    public class Program
    {
        public static IConfiguration Configuration { get; } =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if !DEBUG
                .UseKestrel()
                .UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                .UseUrls("http://*:42420")
                .UseIISIntegration()
#endif
                .UseStartup<Startup>()
                .UseConfiguration(Configuration);
    }
}
