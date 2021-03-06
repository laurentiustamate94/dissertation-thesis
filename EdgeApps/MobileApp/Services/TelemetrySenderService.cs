﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Communication.Common.Models;
using MobileApp.Interfaces;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MobileApp.Services
{
    public class TelemetrySenderService : ITelemetrySenderService
    {
        private const int FOG_APP_PORT = 42420;

#if DEBUG
        private const string PROTOCOL = "https";
#else
        private const string PROTOCOL = "http";
#endif

        private IList<string> Endpoints { get; }

        private IPingService PingService { get; }

        public TelemetrySenderService()
        {
            this.Endpoints = new List<string>();
            this.PingService = DependencyService.Resolve<IPingService>();

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public async Task ScanForFogApps(bool shouldRescan)
        {
            if (!shouldRescan)
            {
                return;
            }

            var ipAddresses = this.PingService.GetReachableHosts();
            using (var httpClient = new HttpClient(new HttpClientHandler()))
            {
                foreach (var ipAddress in ipAddresses)
                {

                    httpClient.BaseAddress = new Uri($"{PROTOCOL}://{ipAddress}:{FOG_APP_PORT}");
                    httpClient.Timeout = TimeSpan.FromMilliseconds(10000);

                    try
                    {
                        var response = await httpClient.SendAsync(new HttpRequestMessage()
                        {
                            RequestUri = new Uri("/api/data"),
                            Method = HttpMethod.Head
                        });

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            this.Endpoints.Add(ipAddress);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public async Task SendData(IEnumerable<DataContract> dataContracts)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler()))
            {
                foreach (var endpoint in this.Endpoints)
                {
                    httpClient.BaseAddress = new Uri($"{PROTOCOL}://{endpoint}:{FOG_APP_PORT}");
                    httpClient.Timeout = TimeSpan.FromMilliseconds(10000);

                    try
                    {
                        var response = await httpClient.PostAsync(
                            "/api/data",
                            new StringContent(JsonConvert.SerializeObject(dataContracts), Encoding.UTF8, "application/json"));

                        Console.WriteLine(response.StatusCode);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
