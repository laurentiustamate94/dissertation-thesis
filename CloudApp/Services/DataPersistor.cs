using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using CloudApp.Interfaces;
using Communication.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.EventHubs;
using Communication.Common.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using CloudApp.DbModels;

namespace CloudApp.Services
{
    public class DataPersistor : IDataPersistor
    {
        private readonly string keyDirectory = null;

        private IConfiguration Configuration { get; }

        private IDataProtector DataProtector { get; }

        public IHttpContextAccessor HttpContextAccessor { get; }

        private EventHubClient EventHubClient { get; }

        public DataPersistor(IConfiguration configuration, IDataProtector dataProtector, IHttpContextAccessor httpContextAccessor, EventHubClient eventHubClient)
        {
            Configuration = configuration;

            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var keyDirectory = Path.Combine(directory, "pairs");

            DataProtector = dataProtector;
            HttpContextAccessor = httpContextAccessor;
            EventHubClient = eventHubClient;
        }

        public async Task<HttpResponseMessage> PersistData(DataContract message)
        {
            //var message = JsonConvert.SerializeObject(new SampleData() { ID = random.Next(), Name = $"Sample Name {random.Next()}" });
            //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
            //await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));


            return new HttpResponseMessage(HttpStatusCode.OK);
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
                var firstFileName = Path.GetFileNameWithoutExtension(Directory.GetFiles(path, "*.private.*").First());
                keysToReturn.Add(firstFileName);
                //keysToReturn.Add(new KeyPairViewModel
                //{
                //    Id = new DirectoryInfo(path).Name,
                //    Purpose = Path.GetFileNameWithoutExtension(firstFileName)
                //});
            }

            return keysToReturn;
        }

        private bool TryDecrypt(DataContract message, out DecryptedData decryptedData)
        {
            decryptedData = null;
            var keyPaths = this.GetAllPgpKeysForUserId(message.UserId);

            foreach (var path in keyPaths)
            {
                var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var inputFilePath = Path.Combine(directory, "message.pgp");
                var outputFilePath = Path.Combine(directory, "message__decrypted");
                File.WriteAllBytes(inputFilePath, Convert.FromBase64String(message.EncryptedData));

                using (var dbContext = HttpContextAccessor.HttpContext.RequestServices.GetService(typeof(DissertationThesisContext)) as DissertationThesisContext)
                {
                    var user = dbContext.Users.FirstOrDefault(x => x.Id == message.UserId);

                    var isSuccessful = DataProtector.DecryptFile(inputFilePath, outputFilePath, path, user.PasswordHash);

                    if (isSuccessful)
                    {
                        decryptedData = JsonConvert.DeserializeObject<DecryptedData>(File.ReadAllText(outputFilePath));

                        File.Delete(inputFilePath);
                        File.Delete(outputFilePath);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
