namespace CloudApp.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string Reason { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public bool ShowReason => !string.IsNullOrEmpty(RequestId);
    }
}