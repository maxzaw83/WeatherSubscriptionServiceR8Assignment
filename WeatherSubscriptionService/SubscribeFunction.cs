using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.Tables;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using WeatherSubscriptionService.Models;

namespace WeatherSubscriptionService
{
    public static class SubscribeFunction
    {
        [FunctionName("SubscribeFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "subscribe")] HttpRequest req,
            [Table("Subscriptions")] IAsyncCollector<SubscriptionEntity> subscriptionTable,
            ILogger log)
        {
            log.LogInformation("Subscribe function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SubscriptionRequest>(requestBody);

            if (data is null || string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.City))
            {
                log.LogError("Invalid subscription request: payload was null or Email/City was missing.");
                return new BadRequestObjectResult("Please provide both a valid email and city.");
            }
            // New: Validate the email format
            try
            {
                var mailAddress = new MailAddress(data.Email);
            }
            catch (FormatException)
            {
                log.LogError($"Invalid email format: {data.Email}");
                return new BadRequestObjectResult("The provided email address is not in a valid format.");
            }

            var subscription = new SubscriptionEntity
            {
                PartitionKey = data.City,
                RowKey = data.Email,
                Email = data.Email,
                City = data.City
            };

            await subscriptionTable.AddAsync(subscription);

            log.LogInformation($"Successfully subscribed {data.Email} for {data.City}.");
            return new OkObjectResult($"Subscription successful for {data.Email} in {data.City}.");
        }
    }
}