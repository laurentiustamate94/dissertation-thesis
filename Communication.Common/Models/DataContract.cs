using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Common.Models
{
    public class DataContract
    {
        public string PacketId { get; set; }

        public string UserId { get; set; }

        public string EncryptedData { get; set; }
    }
}
