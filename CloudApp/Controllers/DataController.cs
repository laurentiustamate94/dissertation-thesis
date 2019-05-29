using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CloudApp.Interfaces;
using Communication.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        public IDataPersistor DataPersistor { get; }

        public DataController(IDataPersistor dataPersistor)
        {
            DataPersistor = dataPersistor;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] DataContract message)
        {
            return await DataPersistor.PersistData(message);
        }

        [HttpHead]
        public HttpResponseMessage Head()
        {
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}
