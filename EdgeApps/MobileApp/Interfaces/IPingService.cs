using System;
using System.Collections.Generic;
using System.Text;

namespace MobileApp.Interfaces
{
    public interface IPingService
    {
        IEnumerable<string> GetReachableHosts();
    }
}
