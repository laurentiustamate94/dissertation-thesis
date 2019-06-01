using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Common.Interfaces;
using Communication.Common.Models;
using MobileApp.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MobileApp.Services
{
    public class TelemetryEncrypterService : ITelemetryEncrypterService
    {
        private IFileAccessor FileAccessor { get; }

        public IUniqueIdGenerationService UniqueIdGenerationService { get; }

        public IDataProtector DataProtector { get; }

        private string RootPath { get; }

        public TelemetryEncrypterService()
        {
            this.RootPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            this.FileAccessor = DependencyService.Get<IFileAccessor>();
            this.DataProtector = DependencyService.Resolve<IDataProtector>();
            this.UniqueIdGenerationService = DependencyService.Resolve<IUniqueIdGenerationService>();
        }

        public Task<IEnumerable<DataContract>> GetDataContracts(IEnumerable<DecryptedData> decryptedData)
        {
            var publicKeyPaths = this.FileAccessor.GetLocalPublicKeyTemporaryPaths();
            var dataContracts = new List<DataContract>();

            foreach (var item in decryptedData)
            {
                var jsonData = JsonConvert.SerializeObject(item);
                var binaryData = Encoding.UTF8.GetBytes(jsonData);

                var inputTempFilePath = Path.Combine(this.RootPath, "temp.bin");
                File.WriteAllBytes(inputTempFilePath, binaryData);

                var outputTempFilePath = this.DataProtector.EncryptFile(inputTempFilePath, publicKeyPaths);
                var encryptedData = File.ReadAllBytes(outputTempFilePath);

                dataContracts.Add(new DataContract()
                {
                    PacketId = UniqueIdGenerationService.GenerateRandomId(),
                    UserId = App.USER_ID,
                    EncryptedData = Convert.ToBase64String(encryptedData)
                });

                File.Delete(inputTempFilePath);
                File.Delete(outputTempFilePath);
            }

            publicKeyPaths.All(p =>
            {
                File.Delete(p);

                return true;
            });

            return Task.FromResult(dataContracts.AsEnumerable());
        }
    }
}
