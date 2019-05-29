using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Common.Interfaces
{
    public interface IDataProtector
    {
        bool DecryptFile(string inputPath, string publicKeyPath, string privateKeyPath, string privateKeyPassword, bool armor = true, bool withIntegrityCheck = true);

        string EncryptFile(string inputTempFilePath, string[] publicKeyPaths, bool armor = true, bool withIntegrityCheck = true);
    }
}
