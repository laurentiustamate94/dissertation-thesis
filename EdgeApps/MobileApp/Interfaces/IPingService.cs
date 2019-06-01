using System.Collections.Generic;

namespace MobileApp.Interfaces
{
    public interface IPingService
    {
        IEnumerable<string> GetReachableHosts();
    }
}
