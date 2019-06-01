using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Common.Models;

namespace MobileApp.Interfaces
{
    public interface ITelemetryEncrypterService
    {
        Task<IEnumerable<DataContract>> GetDataContracts(IEnumerable<DecryptedData> decryptedData);
    }
}
