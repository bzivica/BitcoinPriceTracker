using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using BitcoinApp.Services.Interfaces;

namespace BitcoinApp.Services;

/// <summary>
/// Service for retrieving Bitcoin price in EUR from the CoinDesk API.
/// Implements ICoindeskService to provide an abstraction for fetching Bitcoin prices.
/// </summary>
public class CoindeskService : ICoindeskService
{
    private readonly string _coindeskApiUrl;
    private readonly HttpClient _httpClient;
    private const string CoindDeskURLKey = "AppSettings:CoinDeskApiUrl";

    // Constructor that takes IConfiguration to load the API URL from appsettings.json
    public CoindeskService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _coindeskApiUrl = configuration[CoindDeskURLKey] ?? throw new ArgumentNullException("CoinDeskApiUrl", "API URL must be configured.");
    }

    // Asynchronous method to get the Bitcoin price in EUR
    public async Task<decimal> GetBitcoinPriceInEURAsync()
    {
        try
        {
            // Send a GET request to the API and get the response as a string
            var response = await _httpClient.GetStringAsync(_coindeskApiUrl);

            // Parse the JSON response
            JObject json = JObject.Parse(response);

            // Look for the Bitcoin price in EUR in the response
            var rateToken = json["bpi"]?["EUR"]?["rate_float"];
            if (rateToken == null)
            {
                // If the rate is not found, throw an exception
                throw new Exception("Unable to retrieve Bitcoin price in EUR.");
            }

            // Return the value of the rate as decimal
            return rateToken.Value<decimal>();
        }
        catch (Exception ex)
        {
            // Handle errors, such as issues with the API or network errors
            throw new Exception($"An error occurred while fetching the Bitcoin price in EUR: {ex.Message}", ex);
        }
    }
}