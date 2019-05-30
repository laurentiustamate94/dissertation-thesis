using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CloudApp.DbModels;
using CloudApp.Interfaces;
using Communication.Common;
using Communication.Common.Interfaces;
using Communication.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CloudApp.Services
{
    public class DataPersistor : IDataPersistor
    {
        private IConfiguration Configuration { get; }

        private IDataProtector DataProtector { get; }

        public IHttpContextAccessor HttpContextAccessor { get; }

        private EventHubClient EventHubClient { get; }

        private readonly string keyDirectory = null;

        public DataPersistor(
            IConfiguration configuration,
            IDataProtector dataProtector,
            IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;

            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            keyDirectory = Path.Combine(directory, "pairs");

            DataProtector = dataProtector;
            HttpContextAccessor = httpContextAccessor;
            EventHubClient = EventHubClient.CreateFromConnectionString(Configuration.GetConnectionString("EventHub"));
        }

        public async Task<HttpResponseMessage> PersistData(DataContract[] requestData)
        {
            foreach (var message in requestData)
            {
                DecryptedData data;

                if (this.TryDecrypt(message, out data))
                {
                    await this.HandleDecryptedData(data);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task HandleDecryptedData(DecryptedData data)
        {
            var message = JsonConvert.SerializeObject(
                new
                {
                    DataSource = data.DataSource,
                    PlatformType = data.PlatformType,
                    DataType = data.DataType,
                    CollectedData = DecodeBase64Data(data)
                });

            await EventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
        }

        private object DecodeBase64Data(DecryptedData data)
        {
            if (data.DataType == DataType.Number)
            {
                var number = BitConverter.ToDouble(Convert.FromBase64String(data.Base64Data));

                if (data.DataSource == DataSourceType.Battery)
                {
                    number *= 100;
                }

                return number;
            }

            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(data.Base64Data)));
        }

        private IEnumerable<string> GetAllPgpKeysForUserId(string userId)
        {
            var keysToReturn = new List<string>();
            var directoryPath = Path.Combine(keyDirectory, userId);

            if (!Directory.Exists(directoryPath))
            {
                return keysToReturn;
            }

            var directories = Directory.GetDirectories(directoryPath);

            foreach (var path in directories)
            {
                var firstFileName = Directory.GetFiles(path, "*.private.*").First();
                keysToReturn.Add(firstFileName);
            }

            return keysToReturn;
        }

        private bool TryDecrypt(DataContract message, out DecryptedData decryptedData)
        {
            decryptedData = null;
            var keyPaths = this.GetAllPgpKeysForUserId(message.UserId);

            using (var dbContext = HttpContextAccessor.HttpContext.RequestServices.GetService(typeof(DissertationThesisContext)) as DissertationThesisContext)
            {
                var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var inputFilePath = Path.Combine(directory, "message.pgp");
                var outputFilePath = Path.Combine(directory, "message__decrypted");
                var user = dbContext.Users.FirstOrDefault(x => x.Id == message.UserId);

                foreach (var path in keyPaths)
                {
                    File.WriteAllBytes(inputFilePath, Convert.FromBase64String(message.EncryptedData));

                    var isSuccessful = DataProtector.DecryptFile(inputFilePath, outputFilePath, path, user.PasswordHash);
                    if (isSuccessful)
                    {
                        decryptedData = JsonConvert.DeserializeObject<DecryptedData>(File.ReadAllText(outputFilePath));

                        File.Delete(inputFilePath);
                        File.Delete(outputFilePath);

                        return true;
                    }
                }

                if (File.Exists(inputFilePath))
                {
                    File.Delete(inputFilePath);
                }

                if (File.Exists(outputFilePath))
                {
                    File.Delete(outputFilePath);
                }
            }

            return false;
        }
    }
}
