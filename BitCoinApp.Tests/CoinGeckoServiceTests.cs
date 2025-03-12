using Moq;
using Moq.Contrib.HttpClient;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using BitcoinApp.Services;
using Microsoft.Extensions.Configuration;
using System.Net;

[TestFixture]
public class CoinGeckoServiceTests
{
    private HttpClient? _httpClient;
    private CoinGeckoService? _coinGeckoService;
    private const string CoinGeckoApiUrl = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=eur";

    [SetUp]
    public void SetUp()
    {
        // Create a mocked HttpMessageHandler using Moq.Contrib.HttpClient
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Mock the response for a specific URL
        mockHttpMessageHandler
            .SetupRequest(CoinGeckoApiUrl)
            .ReturnsResponse("{\"bitcoin\": {\"eur\": 50000}}", "application/json");

        // Create an HttpClient based on the mocked handler
        _httpClient = mockHttpMessageHandler.CreateClient();

        // Initialize CoinGeckoService with the mocked HttpClient
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["AppSettings:CoinGeckoApiUrl"]).Returns(CoinGeckoApiUrl);

        _coinGeckoService = new CoinGeckoService(mockConfiguration.Object, _httpClient);
    }

    // TearDown to dispose HttpClient after each test
    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task GetBitcoinPriceInEURAsync_ShouldReturnCorrectPrice()
    {
        if (_coinGeckoService != null)
        {
            // Act: Call the method to fetch the Bitcoin price
            var result = await _coinGeckoService.GetBitcoinPriceInEURAsync();

            // Assert: Verify that the returned price matches the expected value
            Assert.That(result, Is.EqualTo(50000));
        }
        else
        {
            Assert.Fail("CoinGeckoService is not initialized.");
        }
    }

    [Test]
    public async Task GetBitcoinPriceInEURAsync_ShouldThrowException_WhenApiResponseIsMissingPrice()
    {
        // Mock the response without the price data
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .SetupRequest(CoinGeckoApiUrl)
            .ReturnsResponse("{\"bitcoin\": {}}", "application/json");

        // Create an HttpClient based on the mocked handler
        var httpClient = mockHttpMessageHandler.CreateClient();

        // Initialize CoinGeckoService with the new mocked HttpClient
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["AppSettings:CoinGeckoApiUrl"]).Returns(CoinGeckoApiUrl);
        var coinGeckoService = new CoinGeckoService(mockConfiguration.Object, httpClient);

        // Act & Assert: Ensure that an exception is thrown when the price is missing
        var ex = await Task.Run(() => Assert.ThrowsAsync<Exception>(async () =>
            await coinGeckoService.GetBitcoinPriceInEURAsync()));

        // Assert: Check if the exception message matches the expected output
        Assert.That(ex?.Message, Is.EqualTo("An error occurred while fetching the Bitcoin price in EUR from CoinGecko: Unable to retrieve Bitcoin price in EUR."));
    }

    [Test]
    public async Task GetBitcoinPriceInEURAsync_ShouldThrowException_WhenApiThrowsError()
    {
        // Arrange: Mock the HTTP request to throw an exception (e.g., network error)
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .SetupRequest(CoinGeckoApiUrl)
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Create an HttpClient based on the mocked handler
        var httpClient = mockHttpMessageHandler.CreateClient();

        // Initialize CoinGeckoService with the new mocked HttpClient
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(config => config["AppSettings:CoinGeckoApiUrl"]).Returns(CoinGeckoApiUrl);
        var coinGeckoService = new CoinGeckoService(mockConfiguration.Object, httpClient);

        // Act & Assert: Ensure that an exception is thrown when the API fails
        var ex = await Task.Run(() => Assert.ThrowsAsync<Exception>(async () =>
            await coinGeckoService.GetBitcoinPriceInEURAsync()));

        // Assert: Check if the exception message matches the expected output
        Assert.That(ex?.Message, Is.EqualTo("An error occurred while fetching the Bitcoin price in EUR from CoinGecko: Network error"));
    }
}
