namespace Communication.Common.Interfaces
{
    public interface IDataProtector
    {
        void GenerateKey(string publicKeyPath, string privateKeyPath, string email, string password, int strength = 1024, int certainty = 8);

        string EncryptFile(string inputPath, string publicKeyPath, bool armor = true, bool withIntegrityCheck = true);

        string EncryptFile(string inputPath, string[] publicKeyPaths, bool armor = true, bool withIntegrityCheck = true);

        bool DecryptFile(string inputPath, string outputPath, string privateKeyPath, string privateKeyPassword);
    }
}
