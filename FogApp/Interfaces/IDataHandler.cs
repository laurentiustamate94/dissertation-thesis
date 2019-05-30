using System.Threading.Tasks;
using Communication.Common.Models;

namespace FogApp.Interfaces
{
    public interface IDataHandler
    {
        Task Execute(DecryptedData data);
    }
}
