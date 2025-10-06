using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace WeatherSubscriptionService
{
    public static class SendNotificationFunction
    {
        [FunctionName("SendNotificationFunction")]
        public static void Run(
            [QueueTrigger("weather-notifications", Connection = "AzureWebJobsStorage")] string myQueueItem,
            ILogger log)
        {
            // This is the "mock" email send. It just logs the content.
            log.LogWarning($"---- MOCK EMAIL SENT ----\nTo: {myQueueItem}\n-------------------------");
        }
    }
}
