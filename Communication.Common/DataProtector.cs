using System;
using Communication.Common.Interfaces;
using PgpCore;

namespace Communication.Common
{
    public class DataProtector : IDataProtector
    {
        private readonly PGP crypter = null;
        private readonly string encryptedSuffix = "__encrypted.pgp";

        public DataProtector()
        {
            crypter = new PGP();
        }

        public void GenerateKey(string publicKeyPath, string privateKeyPath, string email, string password, int strength = 1024, int certainty = 8)
        {
            TryExecute(() => crypter.GenerateKey(publicKeyPath, privateKeyPath, email, password, strength, certainty));
        }

        public string EncryptFile(string inputPath, string publicKeyPath, bool armor = true, bool withIntegrityCheck = true)
        {
            return this.EncryptFile(inputPath, new string[] { publicKeyPath }, armor, withIntegrityCheck);
        }

        public string EncryptFile(string inputPath, string[] publicKeyPaths, bool armor = true, bool withIntegrityCheck = true)
        {
            var outputPath = $"{inputPath}{encryptedSuffix}";
            var isSuccessful = TryExecute(() => crypter.EncryptFile(inputPath, outputPath, publicKeyPaths, armor, withIntegrityCheck));

            return isSuccessful
                ? outputPath
                : throw new Exception("Could not encrypt file!");
        }

        public bool DecryptFile(string inputPath, string outputPath, string privateKeyPath, string privateKeyPassword)
        {
            return TryExecute(() => crypter.DecryptFile(inputPath, outputPath, privateKeyPath, privateKeyPassword));
        }

        private bool TryExecute(Action action)
        {
            var isSuccessful = true;

            try
            {
                action();
            }
            catch (Exception)
            {
                isSuccessful = false;
            }

            return isSuccessful;
        }
    }
}
