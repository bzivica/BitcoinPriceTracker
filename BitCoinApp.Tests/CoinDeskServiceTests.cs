using Moq;
using Moq.Contrib.HttpClient;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using BitcoinApp.Services;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace BitcoinApp.Tests;

[TestFixture]
public class CoindeskServiceTests
{
    private HttpClient? _httpClient;
    private CoindeskService? _coindeskService;

    private const string CoinDeskApiUrl = "https://api.coindesk.com/v1/bpi/currentprice/BTC.json";

    [SetUp]
    public void SetUp()
    {
        // Mock HttpMessageHandler using Moq.Contrib.HttpClient
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Setup mock response for the CoinDesk API URL
        mockHttpMessageHandler
            .SetupRequest(HttpMethod.Get, CoinDeskApiUrl)
            .ReturnsResponse("{\"bpi\": {\"EUR\": {\"rate_float\": 40000.00}}}", "application/json");

        // Create HttpClient from the mocked handler
        _httpClient = mockHttpMessageHandler.CreateClient();

        // Mock IConfiguration to return CoinDesk API URL from appsettings
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["AppSettings:CoinDeskApiUrl"]).Returns(CoinDeskApiUrl);

        // Initialize CoindeskService with the mocked HttpClient and IConfiguration
        _coindeskService = new CoindeskService(mockConfiguration.Object, _httpClient);
    }

    // TearDown to dispose HttpClient after each test
    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task GetBitcoinPriceInEURAsync_ShouldReturnPrice_WhenApiReturnsValidResponse()
    {
        // Act: Call the method to get the Bitcoin price in EUR
        if (_coindeskService != null)
        {
            var priceInEUR = await _coindeskService.GetBitcoinPriceInEURAsync();

            // Assert: Verify that the returned price is correct
            Assert.That(priceInEUR, Is.EqualTo(40000.00m));
        }
        else
        {
            Assert.Fail("CoindeskService is not initialized.");
        }
    }

    [Test]
    public async Task GetBitcoinPriceInEURAsync_ShouldThrowException_WhenApiResponseIsMissingRate()
    {
        // Arrange: Mock HttpMessageHandler to return a response missing rate
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .SetupRequest(HttpMethod.Get, CoinDeskApiUrl)
            .ReturnsResponse("{\"bpi\": {\"EUR\": {}}}");

        // Create an HttpClient based on the mocked handler
        _httpClient = mockHttpMessageHandler.CreateClient();

        // Mock IConfiguration to return CoinDesk API URL from appsettings
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["AppSettings:CoinDeskApiUrl"]).Returns(CoinDeskApiUrl);

        // Initialize CoindeskService with the new mocked HttpClient
        _coindeskService = new CoindeskService(mockConfiguration.Object, _httpClient);

        // Act & Assert: Ensure that an exception is thrown when the price is missing
        var ex = await Task.Run(() => Assert.ThrowsAsync<Exception>(async () =>
            await _coindeskService.GetBitcoinPriceInEURAsync()));

        // Assert: Verify that the exception message is as expected
        Assert.That(ex?.Message, Is.EqualTo("An error occurred while fetching the Bitcoin price in EUR: Unable to retrieve Bitcoin price in EUR."));
    }

    [Test]
    public async Task GetBitcoinPriceInEURAsync_ShouldThrowException_WhenApiThrowsError()
    {
        // Arrange: Mock HttpClient to throw an exception (e.g., network error)
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .SetupRequest(HttpMethod.Get, CoinDeskApiUrl)
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Create an HttpClient based on the mocked handler
        _httpClient = mockHttpMessageHandler.CreateClient();

        // Mock IConfiguration to return CoinDesk API URL from appsettings
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["AppSettings:CoinDeskApiUrl"]).Returns(CoinDeskApiUrl);

        // Initialize CoindeskService with the new mocked HttpClient
        _coindeskService = new CoindeskService(mockConfiguration.Object, _httpClient);

        // Act & Assert: Verify that an exception is thrown when the API fails
        var ex = await Task.Run(() => Assert.ThrowsAsync<Exception>(async () =>
            await _coindeskService.GetBitcoinPriceInEURAsync()));

        // Assert: Verify that the exception message is as expected
        Assert.That(ex?.Message, Is.EqualTo("An error occurred while fetching the Bitcoin price in EUR: Network error"));
    }
}
