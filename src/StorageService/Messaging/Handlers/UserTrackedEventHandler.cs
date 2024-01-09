namespace StorageService.Messaging.Handlers
{
    using KafkaFlow;

    using Microsoft.Extensions.Options;

    using StorageService.File;
    using StorageService.Settings;
    using StorageService.Visits;

    using TrackingContracts;

    public class UserTrackedEventHandler : IMessageHandler<UserTrackedEvent>
    {
        private readonly StorageSettings storageSettings;
        private readonly IFileWriter fileWriter;

        public UserTrackedEventHandler(
            IOptions<StorageSettings> storageSettings, 
            IFileWriter fileWriter)
        {
            this.storageSettings = storageSettings.Value;
            this.fileWriter = fileWriter;
        }
        
        public Task Handle(IMessageContext context, UserTrackedEvent message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message.IpAddress))
                {
                    // could log a warning
                    return Task.CompletedTask;
                }

                // could use a mapper
                var visitLog = new VisitsLogBuilder()
                    .WithIpAddress(message.IpAddress)
                    .WithReferer(message.Referer)
                    .WithUserAgent(message.UserAgent)
                    // we could also fallback the timestamp value to the current utc time if info in event is invalid
                    .WithTimestamp(message.Timestamp).Build();
            
                // could also throw on application startup if settings file path is invalid
                var visitsFilePath = string.IsNullOrWhiteSpace(this.storageSettings.VisitsLogFilePath) ? 
                    "/tmp/visits.log" : this.storageSettings.VisitsLogFilePath;

                this.fileWriter.AppendLine(visitsFilePath, visitLog.AsLogEntry());

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}