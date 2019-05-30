using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using MobileApp.Interfaces;

namespace MobileApp.Services
{
    public class PingService : IPingService
    {
        private ConcurrentBag<string> ReachableHosts { get; }

        public PingService()
        {
            ReachableHosts = new ConcurrentBag<string>();
        }

        public IEnumerable<string> GetReachableHosts()
        {
            PopulateReachableHosts();

            return ReachableHosts.ToList();
        }

        private void PopulateReachableHosts()
        {
            var currentIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];

            // to make sure it's an IPV4
            if (currentIP.AddressFamily != AddressFamily.InterNetwork)
            {
                return;
            }

            var scanIP = currentIP.ToString().Substring(0, currentIP.ToString().LastIndexOf(".")) + ".";
            var startIp = IPAddress.Parse($"{scanIP}1");
            var endIp = IPAddress.Parse($"{scanIP}254");

            if (Parallel.ForEach(GenerateIpAddressesList(startIp, endIp), CheckIpAddress).IsCompleted)
            {
                Console.WriteLine("Finish pinging hosts!");
            }
        }

        private void CheckIpAddress(IPAddress address)
        {
            try
            {
                var pingStatus = new Ping().Send(address).Status;

                if (pingStatus == IPStatus.Success)
                {
                    if (!ReachableHosts.Any(x => x == address.ToString()))
                    {
                        ReachableHosts.Add(address.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private IEnumerable<IPAddress> GenerateIpAddressesList(IPAddress startIp, IPAddress endIp)
        {
            yield return startIp;
            IPAddress newIp = startIp;

            while (true)
            {
                newIp = IncrementBytesInIp(newIp);

                if (newIp.Equals(endIp))
                {
                    break;
                }

                yield return newIp;
            }

            yield return endIp;
        }

        private IPAddress IncrementBytesInIp(IPAddress ip)
        {
            var bytes = ip.GetAddressBytes();

            if (++bytes[3] == 0)
            {
                if (++bytes[2] == 0)
                {
                    if (++bytes[1] == 0)
                    {
                        ++bytes[0];
                    }
                }
            }

            return new IPAddress(bytes);
        }
    }
}
