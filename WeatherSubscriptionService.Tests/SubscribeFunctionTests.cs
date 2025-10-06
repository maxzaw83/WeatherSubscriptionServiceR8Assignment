using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using WeatherSubscriptionService.Models;

namespace WeatherSubscriptionService.Tests
{
    [TestClass]
    public class SubscribeFunctionTests
    {
        [TestMethod]
        public async Task Run_WithMissingEmail_ReturnsBadRequest()
        {
            // Arrange: Create a mock request with invalid data (missing email)
            var requestData = new { city = "Auckland" };
            var json = JsonConvert.SerializeObject(requestData);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Body).Returns(stream);

            // Mock the other dependencies (they aren't used in the validation path)
            var mockCollector = new Mock<IAsyncCollector<SubscriptionEntity>>();
            var mockLogger = new Mock<ILogger>();

            // Act: Call the function with the mock request
            var result = await SubscribeFunction.Run(
                mockRequest.Object,
                mockCollector.Object,
                mockLogger.Object);

            // Assert: Check that the result is a BadRequestObjectResult
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Run_WithValidRequest_AddsToCollectorAndReturnsOk()
        {
            // Arrange: Create a valid request
            var requestData = new { email = "test@example.com", city = "Auckland" };
            var json = JsonConvert.SerializeObject(requestData);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Body).Returns(stream);

            // Arrange: Set up the table collector to verify that AddAsync is called
            var mockCollector = new Mock<IAsyncCollector<SubscriptionEntity>>();
            var mockLogger = new Mock<ILogger>();

            // Act: Call the function
            var result = await SubscribeFunction.Run(
                mockRequest.Object,
                mockCollector.Object,
                mockLogger.Object);

            // Assert: Check that the result is an OkObjectResult
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            // Assert: Verify that AddAsync was called on the collector exactly once
            mockCollector.Verify(c => c.AddAsync(
                It.Is<SubscriptionEntity>(s => s.RowKey == "test@example.com" && s.PartitionKey == "Auckland"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Run_WithNullBody_ReturnsBadRequest()
        {
            // Arrange: Create a request where the JSON body is null
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Body).Returns(new MemoryStream()); // Empty body

            var mockCollector = new Mock<IAsyncCollector<SubscriptionEntity>>();
            var mockLogger = new Mock<ILogger>();

            // Act: Call the function
            var result = await SubscribeFunction.Run(
                mockRequest.Object,
                mockCollector.Object,
                mockLogger.Object);

            // Assert: Check that the result is a BadRequestObjectResult
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}