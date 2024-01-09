namespace StorageService.Visits
{
    public class VisitsLog
    {
        public string Timestamp { get; set; }
        
        public string IpAddress { get; set; }
        
        public string? Referer { get; set; }
        
        public string? UserAgent { get; set; }

        public string AsLogEntry()
        {
            // Use "null" for null strings instead of empty string
            var referer = string.IsNullOrWhiteSpace(Referer) ? "null" : Referer;
            var userAgent = string.IsNullOrWhiteSpace(UserAgent) ? "null" : UserAgent;
            
            return $@"{Timestamp}|{referer}|{userAgent}|{IpAddress}";
        }
    }
}