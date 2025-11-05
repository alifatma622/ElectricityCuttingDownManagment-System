namespace ElectricityCuttingDown.WebPortal.Models
{
    // Simple view model used by Views/Shared/Error.cshtml
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
