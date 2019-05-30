using System.Collections.Generic;
using System.Security.Claims;
using CloudApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudApp.Interfaces
{
    public interface IKeyPairManagementService
    {
        IEnumerable<KeyPairViewModel> GetAllKeyPairs(IEnumerable<Claim> claims);

        void GenerateKeyPair(IEnumerable<Claim> claims, string keyPurpose);

        void DeleteKeyPair(IEnumerable<Claim> claims, string id);

        FileContentResult DownloadKey(IEnumerable<Claim> claims, string id, string prefix);
    }
}
