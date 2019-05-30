using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Communication.Common.Models;

namespace FogApp.Interfaces
{
    public interface IDataAggregatorService
    {
        IEnumerable<IDataHandler> DataHandlers { get; }

        bool TryDecrypt(DataContract message, out DecryptedData decryptedData);

        Task HandleDecryptedData(DecryptedData data);

        Task<HttpResponseMessage> PersistData(DataContract[] requestData);
    }
}
