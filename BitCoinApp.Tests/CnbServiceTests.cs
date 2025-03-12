using Moq;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using BitcoinApp.Services;
using System;
using Microsoft.Extensions.Configuration;
using Moq.Protected;

namespace BitcoinApp.Tests
{
    [TestFixture]
    public class CnbServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<IConfiguration> _mockConfiguration;
        private CnbService _cnbService;

        private const string CnbApiUrlKey = "AppSettings:CnbApiUrl";

        [SetUp]
        public void Setup()
        {
            // Mock HttpMessageHandler, protože HttpClient nelze přímo mockovat
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Set up mock configuration value for the base API URL
            _mockConfiguration.Setup(c => c[CnbApiUrlKey]).Returns("http://fakeapi.com/");

            // Create HttpClient using the mocked HttpMessageHandler
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            // Initialize the service with the mocked dependencies
            _cnbService = new CnbService(_mockConfiguration.Object, httpClient);
        }

        [Test]
        public async Task GetEurToCzkRateAsync_ShouldReturnValidRate()
        {
            // Arrange: Set up the HttpMessageHandler mock to return a mocked response
            var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"rates\": [{\"currencyCode\": \"EUR\", \"rate\": 25.3}]}") // Mocked JSON response
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<System.Threading.CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act: Call the method under test
            var result = await _cnbService.GetEurToCzkRateAsync();

            // Assert: Check that the result matches the expected rate
            Assert.That(result, Is.EqualTo(25.3m));
        }

        [Test]
        public void GetEurToCzkRateAsync_ShouldThrowException_WhenApiThrowsError()
        {
            // Arrange: Mock the HttpMessageHandler to simulate an exception (e.g., network error)
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<System.Threading.CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Create HttpClient using the mocked handler
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            // Mock IConfiguration to return the base URL
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["AppSettings:CnbApiUrl"]).Returns("http://fakeapi.com/");

            var cnbService = new CnbService(mockConfiguration.Object, httpClient);

            // Act and Assert: Ensure that an exception is thrown when calling GetEurToCzkRateAsync
            Assert.ThrowsAsync<Exception>(async () => await cnbService.GetEurToCzkRateAsync());
        }

        [TearDown]
        public void TearDown()
        {
            _mockHttpMessageHandler?.VerifyAll(); 
            _mockHttpMessageHandler?.Object.Dispose();
        }
    }
}
