using System.Threading.Tasks;
using Communication.Common.Models;

namespace MobileApp.Interfaces
{
    public interface IDataCollector
    {
        Task Start();

        Task Stop();

        Task<DecryptedData> Collect();
    }
}
