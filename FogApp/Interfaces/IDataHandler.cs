using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Communication.Common.Models;

namespace FogApp.Interfaces
{
    public interface IDataHandler
    {
        Task Execute(DecryptedData data);
    }
}
