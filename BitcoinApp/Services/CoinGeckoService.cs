using System;
using System.Net.Http;
using System.Threading.Tasks;
using BitcoinApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace BitcoinApp.Services;

/// <summary>
/// Service for retrieving Bitcoin price in EUR from the CoinGecko API.
/// Implements ICoinGeckoService to provide an abstraction for fetching Bitcoin prices.
/// </summary>
public class CoinGeckoService : ICoinGeckoService
{
    private readonly string _coinGeckoApiUrl;
    private readonly HttpClient _httpClient;
    private const string CoinGeckoApiUrlKey = "AppSettings:CoinGeckoApiUrl";

    // Constructor that takes IConfiguration to load the API URL from appsettings.json
    public CoinGeckoService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _coinGeckoApiUrl = configuration[CoinGeckoApiUrlKey] ?? throw new ArgumentNullException("CoinGeckoApiUrl", "API URL must be configured.");
    }

    // Asynchronous method to get the Bitcoin price in EUR from CoinGecko API
    public async Task<decimal> GetBitcoinPriceInEURAsync()
    {
        try
        {
            // Send a GET request to the CoinGecko API and get the response as a string
            var response = await _httpClient.GetStringAsync(_coinGeckoApiUrl);

            // Parse the JSON response
            JObject json = JObject.Parse(response);

            // Look for the Bitcoin price in EUR in the response
            var priceToken = json["bitcoin"]?["eur"];
            if (priceToken == null)
            {
                // If the price is not found, throw an exception
                throw new Exception("Unable to retrieve Bitcoin price in EUR.");
            }

            // Return the value of the price as decimal
            return priceToken.Value<decimal>();
        }
        catch (Exception ex)
        {
            // Handle errors, such as issues with the API or network errors
            throw new Exception($"An error occurred while fetching the Bitcoin price in EUR from CoinGecko: {ex.Message}", ex);
        }
    }
}
