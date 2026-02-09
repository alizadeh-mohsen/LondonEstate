namespace LondonEstate.Settings
{
    public class UploadSettings
    {
        public int MaxFiles { get; set; } = 5;
        public int MaxFileSizeMB { get; set; } = 7;
        public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        public string EstimationUploadDirectory { get; set; }
        public string AgreementUploadDirectory { get; set; }
        public string InvoiceUploadDirectory { get; set; }
    }
}