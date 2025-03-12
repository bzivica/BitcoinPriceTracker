using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using BitcoinApp.Services;
using BitcoinApp.Services.Interfaces;

namespace BitcoinApp.Tests;

[TestFixture]
public class BitcoinPriceCalculatorServiceTests
{
    private Mock<ICoindeskService> _mockCoinDeskService;
    private Mock<ICnbService> _mockCnbService;
    private Mock<ICoinGeckoService> _mockCoinGeckoService;
    private BitcoinPriceCalculatorService _bitcoinPriceCalculatorService;

    [SetUp]
    public void SetUp()
    {
        // Mocking the services (interfaces)
        _mockCoinDeskService = new Mock<ICoindeskService>();
        _mockCnbService = new Mock<ICnbService>();
        _mockCoinGeckoService = new Mock<ICoinGeckoService>();

        // Create instance of BitcoinPriceCalculatorService using the mocked services
        _bitcoinPriceCalculatorService = new BitcoinPriceCalculatorService(
            _mockCoinDeskService.Object,
            _mockCnbService.Object,
            _mockCoinGeckoService.Object
        );
    }

    [Test]
    public async Task GetBitcoinPriceAsync_ShouldReturnPriceInEURAndCZK_WhenAllServicesWorkProperly()
    {
        // Arrange
        decimal expectedBitcoinPriceEUR = 40000.00m;
        decimal expectedEurToCzkRate = 25.0m;
        decimal expectedBitcoinPriceCZK = expectedBitcoinPriceEUR * expectedEurToCzkRate;

        // Setup the CoinDesk service to return the Bitcoin price in EUR
        _mockCoinDeskService.Setup(service => service.GetBitcoinPriceInEURAsync())
            .ReturnsAsync(expectedBitcoinPriceEUR);

        // Setup CNB service to return EUR to CZK rate
        _mockCnbService.Setup(service => service.GetEurToCzkRateAsync())
            .ReturnsAsync(expectedEurToCzkRate);

        // Act
        var (bitcoinPriceEUR, bitcoinPriceCZK) = await _bitcoinPriceCalculatorService.GetBitcoinPriceAsync();

        // Assert
        Assert.That(bitcoinPriceEUR, Is.EqualTo(expectedBitcoinPriceEUR));
        Assert.That(bitcoinPriceCZK, Is.EqualTo(expectedBitcoinPriceCZK));
    }

    [Test]
    public async Task GetBitcoinPriceAsync_ShouldFallbackToCoinGecko_WhenCoinDeskFails()
    {
        // Arrange
        decimal expectedBitcoinPriceEUR = 40000.00m;
        decimal expectedEurToCzkRate = 25.0m;
        decimal expectedBitcoinPriceCZK = expectedBitcoinPriceEUR * expectedEurToCzkRate;

        // Setup CoinDesk to throw an exception
        _mockCoinDeskService.Setup(service => service.GetBitcoinPriceInEURAsync())
            .ThrowsAsync(new Exception("CoinDesk failed"));

        // Setup CoinGecko to return the Bitcoin price in EUR
        _mockCoinGeckoService.Setup(service => service.GetBitcoinPriceInEURAsync())
            .ReturnsAsync(expectedBitcoinPriceEUR);

        // Setup CNB to return the EUR to CZK exchange rate
        _mockCnbService.Setup(service => service.GetEurToCzkRateAsync())
            .ReturnsAsync(expectedEurToCzkRate);

        // Act
        var (bitcoinPriceEUR, bitcoinPriceCZK) = await _bitcoinPriceCalculatorService.GetBitcoinPriceAsync();

        // Assert
        Assert.That(bitcoinPriceEUR, Is.EqualTo(expectedBitcoinPriceEUR));
        Assert.That(bitcoinPriceCZK, Is.EqualTo(expectedBitcoinPriceCZK));
    }

    [Test]
    public void GetBitcoinPriceAsync_ShouldThrowException_WhenBothCoinDeskAndCoinGeckoFail()
    {
        // Arrange
        _mockCoinDeskService.Setup(service => service.GetBitcoinPriceInEURAsync())
            .ThrowsAsync(new Exception("CoinDesk failed"));

        _mockCoinGeckoService.Setup(service => service.GetBitcoinPriceInEURAsync())
            .ThrowsAsync(new Exception("CoinGecko failed"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _bitcoinPriceCalculatorService.GetBitcoinPriceAsync());
        Assert.That(ex.Message, Is.EqualTo("Both Coindesk and CoinGecko services failed to provide Bitcoin price."));
    }

    [Test]
    public async Task GetBitcoinPriceAsync_ShouldHandleCnbFailureGracefully_WhenCnbFails()
    {
        // Arrange
        decimal expectedBitcoinPriceEUR = 40000.00m;
        decimal expectedEurToCzkRate = 0m;

        // Setup CoinDesk to return the Bitcoin price in EUR
        _mockCoinDeskService.Setup(service => service.GetBitcoinPriceInEURAsync())
            .ReturnsAsync(expectedBitcoinPriceEUR);

        // Setup CNB to throw an exception
        _mockCnbService.Setup(service => service.GetEurToCzkRateAsync())
            .ThrowsAsync(new Exception("CNB failed"));

        // Act
        var (bitcoinPriceEUR, bitcoinPriceCZK) = await _bitcoinPriceCalculatorService.GetBitcoinPriceAsync();

        // Assert
        Assert.That(bitcoinPriceEUR, Is.EqualTo(expectedBitcoinPriceEUR));
        Assert.That(bitcoinPriceCZK, Is.EqualTo(expectedEurToCzkRate)); // CNB failure should result in CZK = 0
    }

    [Test]
    public async Task GetBitcoinPriceAsync_ShouldReturnValidPricesEvenIfCnbFails()
    {
        // Arrange
        decimal expectedBitcoinPriceEUR = 40000.00m;

        // Setup CoinDesk to return the Bitcoin price in EUR
        _mockCoinDeskService.Setup(service => service.GetBitcoinPriceInEURAsync())
            .ReturnsAsync(expectedBitcoinPriceEUR);

        // Setup CNB to throw an exception
        _mockCnbService.Setup(service => service.GetEurToCzkRateAsync())
            .ThrowsAsync(new Exception("CNB failed"));

        // Act
        var (bitcoinPriceEUR, bitcoinPriceCZK) = await _bitcoinPriceCalculatorService.GetBitcoinPriceAsync();

        // Assert
        Assert.That(bitcoinPriceEUR, Is.EqualTo(expectedBitcoinPriceEUR));
        Assert.That(bitcoinPriceCZK, Is.EqualTo(0)); // CNB failure should not affect EUR price
    }
}
