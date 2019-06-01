namespace Communication.Common.Models
{
    public class DecryptedData
    {
        public DataSourceType DataSource { get; set; }

        public DataType DataType { get; set; }

        public PlatformType PlatformType { get; set; }

        public string Base64Data { get; set; }
    }
}
