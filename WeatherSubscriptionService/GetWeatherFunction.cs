using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherSubscriptionService.Models;

namespace WeatherSubscriptionService
{
    public class GetWeatherFunction
    {
        private readonly HttpClient _httpClient;

        public GetWeatherFunction(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }



        [FunctionName("GetWeatherFunction")]
        public async Task Run(
    //[TimerTrigger("0 * * * * *")] TimerInfo myTimer,
    [TimerTrigger("0 0 7 * * *")] TimerInfo myTimer,
    [Table("Subscriptions")] TableClient subscriptionTable,
    [Queue("weather-notifications")] IAsyncCollector<string> notificationQueue, 
    ILogger log)
        {
            log.LogInformation($"GetWeatherFunction executed at: {DateTime.Now}");

            var apiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");
            if (string.IsNullOrEmpty(apiKey))
            {
                log.LogError("OpenWeatherMapApiKey is not set in local.settings.json.");
                return;
            }

            //  stream subscribers from the table
            await foreach (var subscriber in subscriptionTable.QueryAsync<SubscriptionEntity>())
            {
                var apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={subscriber.City}&appid={apiKey}&units=metric";
                try
                {
                    //  wait for the API response
                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        //  get the content
                        string weatherInfo = await response.Content.ReadAsStringAsync();

                        var notificationPayload = new { subscriber.Email, subscriber.City, Weather = weatherInfo };

                        //  add to the queue
                        await notificationQueue.AddAsync(JsonConvert.SerializeObject(notificationPayload));

                        log.LogInformation($"Queued notification for {subscriber.Email} in {subscriber.City}.");
                    }
                    else
                    {
                        log.LogError($"Failed to fetch weather for {subscriber.City}. Status: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Exception fetching weather for {subscriber.City}.");
                }
            }
        }

        //[FunctionName("GetWeatherFunction")] // Remove synchronous version
        //public void Run( 
        //    [TimerTrigger("0 * * * * *")] TimerInfo myTimer,
        //    [Table("Subscriptions")] TableClient subscriptionTable,
        //    [Queue("weather-notifications")] ICollector<string> notificationQueue, 
        //    ILogger log)
        //{
        //    log.LogInformation($"GetWeatherFunction executed at: {DateTime.Now}");

        //    var apiKey = Environment.GetEnvironmentVariable("OpenWeatherMapApiKey");
        //    if (string.IsNullOrEmpty(apiKey))
        //    {
        //        log.LogError("OpenWeatherMapApiKey is not set in local.settings.json.");
        //        return;
        //    }

        //    // Get all subscribers from the table
        //    var subscribers = subscriptionTable.Query<SubscriptionEntity>();

        //    foreach (var subscriber in subscribers)
        //    {
        //        var apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={subscriber.City}&appid={apiKey}&units=metric";
        //        try
        //        {
        //            // Synchronously wait for the API response
        //            HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // Synchronously get the content
        //                string weatherInfo = response.Content.ReadAsStringAsync().Result;

        //                var notificationPayload = new { subscriber.Email, subscriber.City, Weather = weatherInfo };

        //                // Synchronously add to the queue
        //                notificationQueue.Add(JsonConvert.SerializeObject(notificationPayload));

        //                log.LogInformation($"Queued notification for {subscriber.Email} in {subscriber.City}.");
        //            }
        //            else
        //            {
        //                log.LogError($"Failed to fetch weather for {subscriber.City}. Status: {response.StatusCode}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // AggregateException can happen with .Result, so we log the inner exception
        //            log.LogError(ex.InnerException ?? ex, $"Exception fetching weather for {subscriber.City}.");
        //        }
        //    }
        //}
    }
}