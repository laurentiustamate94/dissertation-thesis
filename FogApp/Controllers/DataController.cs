using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Communication.Common.Models;
using Communication.Common.Services;
using FogApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private IDataAggregatorService DataAggregator { get; }

        public DataController(IDataAggregatorService dataAggregator)
        {
            this.DataAggregator = dataAggregator;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] DataContract[] requestData)
        {
            foreach (var message in requestData)
            {
                DecryptedData data;

                if (this.DataAggregator.TryDecrypt(message, out data))
                {
                    await this.DataAggregator.HandleDecryptedData(data);
                }
            }

            return await this.DataAggregator.PersistData(requestData);
        }

        [HttpHead]
        public HttpResponseMessage Head()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
