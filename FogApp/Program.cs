using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FogApp
{
    public class SampleData
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Program
    {
        public static IConfiguration Configuration { get; } =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        public static void Main(string[] args)
        {
            //Console.WriteLine("Starting sending messages to Azure event hub");
            //isRunning = true;
            ////SendingRandomMessages();
            //Console.ReadLine();
            //isRunning = false;
            //Console.WriteLine("Stopping sending messages. Press any key to exit!");
            //Console.ReadLine();

            CreateWebHostBuilder(args).Build().Run();
        }


        // Update connection string : "AZURE_EVENT_HUB_CONNECTION_STRING;EntityPath=EventHubName"
        static string connectionString = "__CONNECTION_STRING__";
        static bool isRunning;

        async static Task SendingRandomMessages()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
            Random random = new Random(1);

            while (isRunning)
            {
                try
                {
                    var message = JsonConvert.SerializeObject(new SampleData() { ID = random.Next(), Name = $"Sample Name {random.Next()}" });
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                //Thread.Sleep(500);
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if !DEBUG
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://*:42420")
                .UseIISIntegration()
#endif
                .UseStartup<Startup>()
                .UseConfiguration(Configuration);
    }
}
