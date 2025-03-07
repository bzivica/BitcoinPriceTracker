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
        private readonly string cnbApiUrl = "https://api.cnb.cz/kurzy/v1/meny.json"; 
        public async Task<decimal> GetEurToCzkRateAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(cnbApiUrl);
                JObject json = JObject.Parse(response);

                var rates = json["rates"];
                if (rates == null)
                {
                    throw new Exception("Rates not found in the response.");
                }

                var eur = rates["EUR"];
                if (eur == null)
                {
                    throw new Exception("EUR rate not found in the response.");
                }

                var rate = eur["rate"];
                if (rate == null)
                {
                    throw new Exception("Rate value not found in the response.");
                }

                return rate.Value<decimal>();
            }
        }
    }
}
