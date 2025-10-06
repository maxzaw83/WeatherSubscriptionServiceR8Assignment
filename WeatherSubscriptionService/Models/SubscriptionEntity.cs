using System;
using Azure.Data.Tables;
using Azure;

namespace WeatherSubscriptionService.Models
{
    // Implement the ITableEntity interface
    public class SubscriptionEntity : ITableEntity
    {
        // Your custom properties
        public string Email { get; set; }
        public string City { get; set; }

        // Properties required by ITableEntity
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}