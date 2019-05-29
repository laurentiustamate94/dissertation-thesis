using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Communication.Common.Services;
using Microsoft.AspNetCore.Mvc;
using PgpCore;
using CloudApp.Models;

namespace CloudApp.Services
{
    public class KeyPairManagementService
    {
        private readonly PGP generator = null;
        private readonly string keyDirectory = null;

        public static KeyPairManagementService Instance { get; set; } = new KeyPairManagementService();

        private KeyPairManagementService()
        {
            generator = new PGP();

            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            keyDirectory = Path.Combine(directory, "pairs");
        }

        public IEnumerable<KeyPairViewModel> GetAllKeyPairs(IEnumerable<Claim> claims)
        {
            var keysToReturn = new List<KeyPairViewModel>();
            var userId = claims.First(x => x.Type == "id").Value;
            var directoryPath = Path.Combine(keyDirectory, userId);

            if(!Directory.Exists(directoryPath))
            {
                return keysToReturn;
            }

            var directories = Directory.GetDirectories(directoryPath);

            foreach (var path in directories)
            {
                var firstFileName = Path.GetFileNameWithoutExtension(Directory.GetFiles(path).First());
                keysToReturn.Add(new KeyPairViewModel
                {
                    Id = new DirectoryInfo(path).Name,
                    Purpose = Path.GetFileNameWithoutExtension(firstFileName)
                });
            }

            return keysToReturn;
        }

        public FileContentResult DownloadKey(IEnumerable<Claim> claims, string id, string prefix)
        {
            var userId = claims.First(x => x.Type == "id").Value;
            var directoryPath = Path.Combine(keyDirectory, userId, id);

            var files = Directory.GetFiles(directoryPath, $"*.{prefix}.*");
            var fileBytes = File.ReadAllBytes(files.First());

            return new FileContentResult(fileBytes, "application/force-download")
            {
                FileDownloadName = Path.GetFileName(files.First())
            };
        }

        public void DeleteKeyPair(IEnumerable<Claim> claims, string id)
        {
            var userId = claims.First(x => x.Type == "id").Value;
            var directoryPath = Path.Combine(keyDirectory, userId, id);

            Directory.Delete(directoryPath, recursive: true);
        }

        public void GenerateKeyPair(IEnumerable<Claim> claims, string keyPurpose)
        {
            var userId = claims.First(x => x.Type == "id").Value;
            var keyId = new UniqueIdGenerationService().GenerateRandomId();
            var directoryPath = Path.Combine(keyDirectory, userId, keyId);
            var filePath = Path.Combine(directoryPath, keyPurpose);

            if (Directory.Exists(directoryPath))
            {
                GenerateKeyPair(claims, keyPurpose);

                return;
            }

            Directory.CreateDirectory(directoryPath);
            generator.GenerateKey(
                $"{filePath}.public.asc",
                $"{filePath}.private.asc",
                claims.First(x => x.Type == "user").Value,
                claims.First(x => x.Type == "hash").Value);
        }
    }
}
