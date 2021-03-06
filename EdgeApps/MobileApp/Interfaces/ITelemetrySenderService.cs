﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Communication.Common.Models;

namespace MobileApp.Interfaces
{
    public interface ITelemetrySenderService
    {
        Task SendData(IEnumerable<DataContract> dataContracts);

        Task ScanForFogApps(bool shouldRescan);
    }
}
