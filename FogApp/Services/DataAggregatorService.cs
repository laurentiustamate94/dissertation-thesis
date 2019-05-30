using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Communication.Common.Interfaces;
using Communication.Common.Models;
using FogApp.Interfaces;
using FogApp.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FogApp.Services
{
    public class DataAggregatorService : IDataAggregatorService
    {
        public IEnumerable<IDataHandler> DataHandlers { get; }

        private IConfiguration Configuration { get; }

        private IHttpClientFactory HttpClientFactory { get; }

        private IDataProtector DataProtector { get; }

        public DataAggregatorService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IDataProtector dataProtector)
        {
            this.Configuration = configuration;
            this.HttpClientFactory = httpClientFactory;
            this.DataProtector = dataProtector;

            this.DataHandlers = this.GetDataHandlers();
        }

        public bool TryDecrypt(DataContract message, out DecryptedData decryptedData)
        {
            decryptedData = null;
            var keyPaths = this.GetAllPgpKeyPaths();

            foreach (var path in keyPaths)
            {
                var firstFileName = Path.GetFileNameWithoutExtension(path);
                var file = this.Configuration.GetSection("KeyCredentials")
                    .Get<PrivateKeyCredentials[]>()
                    .FirstOrDefault(k => k.Purpose == Path.GetFileNameWithoutExtension(firstFileName));

                if (file == null)
                {
                    continue;
                }

                var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var inputFilePath = Path.Combine(directory, "message.pgp");
                var outputFilePath = Path.Combine(directory, "message__decrypted");
                File.WriteAllBytes(inputFilePath, Convert.FromBase64String(message.EncryptedData));

                var isSuccessful = DataProtector.DecryptFile(inputFilePath, outputFilePath, path, file.Password);

                if (isSuccessful)
                {
                    decryptedData = JsonConvert.DeserializeObject<DecryptedData>(File.ReadAllText(outputFilePath));

                    File.Delete(inputFilePath);
                    File.Delete(outputFilePath);

                    return true;
                }
            }

            return false;
        }

        public async Task HandleDecryptedData(DecryptedData data)
        {
            foreach (var handler in DataHandlers)
            {
                await handler.Execute(data);
            }
        }

        public async Task<HttpResponseMessage> PersistData(DataContract[] requestData)
        {
            var httpClient = this.HttpClientFactory.CreateClient();
            var cloudEndpoint = this.Configuration.GetSection("CloudEndpoint").Get<string>();
            httpClient.BaseAddress = new Uri(cloudEndpoint);

            return await httpClient.PostAsync(
                "/api/data", 
                new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json"));
        }

        private IEnumerable<IDataHandler> GetDataHandlers()
        {
            var registeredDataHandlers = this.Configuration.GetSection("DataHandlers").Get<string[]>();

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.Contains("Microsoft") && !a.FullName.Contains("System"))
                .SelectMany(a =>
                {
                    return a.GetTypes()
                        .Where(t => t.IsClass)
                        .Where(t => registeredDataHandlers.Contains(t.Name));
                })
                .Select(t => Activator.CreateInstance(t) as IDataHandler)
                .ToList();
        }

        private IEnumerable<string> GetAllPgpKeyPaths()
        {
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var keyDirectory = Path.Combine(directory, "PgpKeys");

            return Directory.GetFiles(keyDirectory);
        }
    }
}
