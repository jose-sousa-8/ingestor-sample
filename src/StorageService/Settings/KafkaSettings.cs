namespace StorageService.Settings
{
    public class KafkaSettings
    {
        public string[] Brokers { get; set; }
        
        public string ConsumerGroupId { get; set; }
    }
}