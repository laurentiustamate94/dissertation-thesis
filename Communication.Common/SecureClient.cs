using System;
using System.Collections.Generic;
using System.IO;
using Communication.Common.Interfaces;
using PgpCore;

namespace Communication.Common
{
    public class SecureClient : IDataProtector
    {
        private readonly PGP crypter = null;
        private readonly string encryptedSuffix = "__encrypted.pgp";
        private readonly string encryptedSignedSuffix = "__encrypted__signed.pgp";

        public SecureClient()
        {
            crypter = new PGP();
        }

        public string EncryptFile(string inputPath, string publicKeyPath, bool armor = true, bool withIntegrityCheck = true)
        {
            return this.EncryptFile(inputPath, new string[] { publicKeyPath }, armor, withIntegrityCheck);
        }

        public string EncryptFile(string inputPath, string[] publicKeyPaths, bool armor = true, bool withIntegrityCheck = true)
        {
            crypter.EncryptFile(inputPath, $"{inputPath}{encryptedSuffix}", publicKeyPaths, armor, withIntegrityCheck);

            return $"{inputPath}{encryptedSuffix}";
        }

        public void EncryptStream(Stream inputStream, Stream outputStream, IEnumerable<Stream> publicKeyStreams, bool armor = true, bool withIntegrityCheck = true)
        {
            crypter.EncryptStream(inputStream, outputStream, publicKeyStreams, armor, withIntegrityCheck);
        }

        public void EncryptFileWithSignature(string inputPath, string publicKeyPath, string privateKeyPath, string privateKeyPassword, bool armor = true, bool withIntegrityCheck = true)
        {
            crypter.EncryptFileAndSign(inputPath, $"{inputPath}{encryptedSignedSuffix}", publicKeyPath, privateKeyPath, privateKeyPassword, armor, withIntegrityCheck);
        }

        public bool DecryptFile(string inputPath, string publicKeyPath, string privateKeyPath, string privateKeyPassword, bool armor = true, bool withIntegrityCheck = true)
        {
            crypter.DecryptFile(inputPath, publicKeyPath, privateKeyPath, privateKeyPassword);

            return true;
        }

        //public bool DecryptFile(string inputPath, string publicKeyPath, string privateKeyPath, string privateKeyPassword, bool armor = true, bool withIntegrityCheck = true)
        //{
        //    crypter.DecryptFile(@"C:\TEMP\keys\content__encrypted.pgp", @"C:\TEMP\keys\content__decrypted.txt", @"C:\TEMP\keys\2.private.asc", "password");

        //    return true;
        //}

        public void DecryptSignedFile()
        {

            crypter.DecryptFile(@"C:\TEMP\keys\content__encrypted_signed.pgp", @"C:\TEMP\keys\content__decrypted_signed.txt", @"C:\TEMP\keys\1.private.asc", "password");
        }

        public void EncryptStream()
        {

        }

        public void DecryptStream()
        {

        }

        //using (PGP pgp = new PGP())
        //{
        //    // Generate keys
        //    pgp.GenerateKey(@"C:\TEMP\keys\1.public.asc", @"C:\TEMP\keys\1.private.asc", "email@email.com", "password");
        //    pgp.GenerateKey(@"C:\TEMP\keys\2.public.asc", @"C:\TEMP\keys\2.private.asc", "email@email.com", "password");
        //    // Encrypt file
        //    pgp.EncryptFile(@"C:\TEMP\keys\content.txt", @"C:\TEMP\keys\content__encrypted.pgp", @"C:\TEMP\keys\1.public.asc", true, true);
        //    // Encrypt file with multiple keys
        //    string[] publicKeys = Directory.GetFiles(@"C:\TEMP\keys\", "*.public.asc");
        //    pgp.EncryptFile(@"C:\TEMP\keys\content.txt", @"C:\TEMP\keys\content__encrypted.pgp", publicKeys, true, true);
        //    // Encrypt and sign file
        //    pgp.EncryptFileAndSign(@"C:\TEMP\keys\content.txt", @"C:\TEMP\keys\content__encrypted_signed.pgp", @"C:\TEMP\keys\1.public.asc", @"C:\TEMP\keys\1.private.asc", "password", true, true);
        //    // Decrypt file
        //    pgp.DecryptFile(@"C:\TEMP\keys\content__encrypted.pgp", @"C:\TEMP\keys\content__decrypted.txt", @"C:\TEMP\keys\2.private.asc", "password");
        //    // Decrypt signed file
        //    pgp.DecryptFile(@"C:\TEMP\keys\content__encrypted_signed.pgp", @"C:\TEMP\keys\content__decrypted_signed.txt", @"C:\TEMP\keys\1.private.asc", "password");

        //    // Encrypt stream
        //    using (FileStream inputFileStream = new FileStream(@"C:\TEMP\keys\content.txt", FileMode.Open))
        //    using (Stream outputFileStream = System.IO.File.Create(@"C:\TEMP\keys\content__encrypted2.pgp"))
        //    using (Stream publicKeyStream = new FileStream(@"C:\TEMP\keys\1.public.asc", FileMode.Open))
        //        pgp.EncryptStream(inputFileStream, outputFileStream, publicKeyStream, true, true);

        //    // Decrypt stream
        //    using (FileStream inputFileStream = new FileStream(@"C:\TEMP\keys\content__encrypted2.pgp", FileMode.Open))
        //    using (Stream outputFileStream = System.IO.File.Create(@"C:\TEMP\keys\content__decrypted2.txt"))
        //    using (Stream privateKeyStream = new FileStream(@"C:\TEMP\keys\1.private.asc", FileMode.Open))
        //        pgp.DecryptStream(inputFileStream, outputFileStream, privateKeyStream, "password");
        //}
    }
}
