namespace LondonEstate.Settings
{
    public class UploadSettings
    {
        public int MaxFiles { get; set; } = 5;
        public int MaxFileSizeMB { get; set; } = 5;
        public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        public string UploadFolder { get; set; } = "uploads";
    }
}