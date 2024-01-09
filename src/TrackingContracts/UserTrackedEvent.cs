namespace TrackingContracts
{
    using System;
    
    public class UserTrackedEvent
    {
        public DateTimeOffset Timestamp { get; set; }
        
        public string IpAddress { get; private set; }
        
        public string? Referer { get; set; }
        
        public string? UserAgent { get; set; }

        public UserTrackedEvent(
            string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentException("ip address must have a value");
            }

            this.IpAddress = ipAddress;
        }
    }
}