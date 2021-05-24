namespace ChartsWebAPI.Models
{
    public class AppSettings
    {
        public string AllowedHosts { get; set; }
        public AzureAd AzureAd { get; set; }
        public ExportServer ExportServer { get; set; }
        public Logging Logging { get; set; }
    }

    public class AzureAd
    {
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Domain { get; set; }
        public string Instance { get; set; }
        public string TenantId { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
        public Console Console { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string System { get; set; }
    }

    public class Console
    {
        public bool IncludeScopes { get; set; }
    }

    public class ExportServer
    {
        public string Private { get; set; }
        public string Public { get; set; }
    }
}
