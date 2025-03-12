using BitcoinApp.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Services;

/// <summary>
/// Service to calculate Bitcoin price in EUR and CZK.
/// Fetches Bitcoin price from multiple sources (Coindesk, CoinGecko) and converts it to CZK using the EUR to CZK exchange rate.
/// </summary>
public class BitcoinPriceCalculatorService
{
    private readonly ICoindeskService _coinDeskService;
    private readonly ICnbService _cnbService;
    private readonly ICoinGeckoService _coinGeckoService;

    // Constructor accepting services that will be used to get Bitcoin price and EUR to CZK exchange rate
    public BitcoinPriceCalculatorService(ICoindeskService coindeskService, ICnbService cnbService, ICoinGeckoService coinGeckoService)
    {
        _coinDeskService = coindeskService;
        _coinGeckoService = coinGeckoService;
        _cnbService = cnbService;
    }

    // This method fetches Bitcoin price in EUR and converts it to CZK using the exchange rate
    public async Task<(decimal bitcoinPriceEUR, decimal bitcoinPriceCZK)> GetBitcoinPriceAsync()
    {
        decimal bitcoinPriceEUR;
        decimal bitcoinPriceCZK = 0;

        try
        {
            // Attempt to fetch Bitcoin price in EUR from Coindesk service
            bitcoinPriceEUR = await _coinDeskService.GetBitcoinPriceInEURAsync();
        }
        catch (Exception ex)
        {
            // Log the error (consider using a logging framework)
            Console.WriteLine($"Coindesk service error: {ex.Message}");

            try
            {
                // If Coindesk fails, attempt to fetch Bitcoin price from CoinGecko service
                bitcoinPriceEUR = await _coinGeckoService.GetBitcoinPriceInEURAsync();
            }
            catch (Exception innerEx)
            {
                // Log the error
                Console.WriteLine($"CoinGecko service error: {innerEx.Message}");
                // Rethrow or handle as appropriate
                throw new Exception("Both Coindesk and CoinGecko services failed to provide Bitcoin price.");
            }
        }

        try
        {
            // Fetch EUR to CZK exchange rate from Cnb service
            decimal eurToCzkRate = await _cnbService.GetEurToCzkRateAsync();

            // Calculate Bitcoin price in CZK
            bitcoinPriceCZK = bitcoinPriceEUR * eurToCzkRate;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Cnb service error: {ex.Message}");
            // Handle as appropriate, e.g., set bitcoinPriceCZK to a default value or rethrow
        }

        // Return a tuple with both prices (EUR and CZK)
        return (bitcoinPriceEUR, bitcoinPriceCZK);
    }
}
