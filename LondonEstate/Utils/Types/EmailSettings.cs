namespace LondonEstate.Utils.Types
{
    public class EmailSettings
    {
        public string FromName { get; set; } = default!;
        public string FromEmail { get; set; } = default!;
        public string ToEmail { get; set; } = default!;
        public string SmtpServer { get; set; } = default!;
        public int Port { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? CcEmail { get; internal set; }
        //public bool UseSsl { get; set; }
        //public bool UseStartTls { get; set; }
        //public byte TimeoutSeconds { get; set; }
    }
}
