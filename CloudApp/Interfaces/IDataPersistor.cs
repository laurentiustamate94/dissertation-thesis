using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Communication.Common.Models;

namespace CloudApp.Interfaces
{
    public interface IDataPersistor
    {
        Task<HttpResponseMessage> PersistData(DataContract message);
    }
}
