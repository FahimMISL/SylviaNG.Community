namespace SylviaNG.Community.Infrastructure.Kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = "sylviang-community-employee-sync";
    }
}
