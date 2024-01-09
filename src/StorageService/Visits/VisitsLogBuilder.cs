namespace StorageService.Visits
{
    public class VisitsLogBuilder
    {
        private readonly VisitsLog visitsLog = new VisitsLog();

        public VisitsLogBuilder WithIpAddress(string ipAddress)
        {
            this.visitsLog.IpAddress = ipAddress;
            return this;
        }
        
        /// <summary>
        /// Sets the visit log timestamp to UTC and ISO 8601 format
        /// </summary>
        /// <param name="timestamp"></param>
        public VisitsLogBuilder WithTimestamp(DateTimeOffset timestamp)
        { 
            this.visitsLog.Timestamp = timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return this;
        }

        public VisitsLogBuilder WithUserAgent(string? userAgent)
        {
            this.visitsLog.UserAgent = userAgent;
            return this;
        }

        public VisitsLogBuilder WithReferer(string? referer)
        {
            this.visitsLog.Referer = referer;
            return this;
        }
        
        public VisitsLog Build()
        {
            if (string.IsNullOrWhiteSpace(this.visitsLog.IpAddress))
            {
                throw new ArgumentException("Visit log must have an ip address");
            }
            
            return this.visitsLog;
        }
    }
}