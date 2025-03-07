using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BitcoinApp.Services
{

    public class CnbService
    {
        private readonly string baseApiUrl = "https://api.cnb.cz/cnbapi/exrates/daily?date=";
        //private readonly string cnbApiUrl = "https://api.cnb.cz/cnbapi/exrates/daily?date=2025-03-07"; 
        public async Task<decimal> GetEurToCzkRateAsync()
        {
            string cnbApiUrl = $"{baseApiUrl}{DateTime.Now.ToString("yyyy-MM-dd")}";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(cnbApiUrl);
                JObject json = JObject.Parse(response);

                var rates = json["rates"];
                if (rates == null)
                {
                    throw new Exception("Rates not found in the response.");
                }

                // Hledání měny EUR v poli rates
                var eurRate = rates.FirstOrDefault(rate => rate["currencyCode"]?.ToString() == "EUR");
                if (eurRate == null)
                {
                    throw new Exception("EUR rate not found in the response.");
                }

                var rate = eurRate["rate"];
                if (rate == null)
                {
                    throw new Exception("Rate value not found in the response.");
                }

                return rate.Value<decimal>();
            }
        }
    }
}
