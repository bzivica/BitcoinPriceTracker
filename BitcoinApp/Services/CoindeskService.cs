using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BitcoinApp.Services
{
    public class CoindeskService
    {
        private readonly string coindeskApiUrl = "https://api.coindesk.com/v1/bpi/currentprice.json";

        public async Task<decimal> GetBitcoinPriceInEURAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(coindeskApiUrl);
                JObject json = JObject.Parse(response);

                var rateToken = json["bpi"]?["EUR"]?["rate_float"];
                if (rateToken == null)
                {
                    throw new Exception("Unable to retrieve Bitcoin price in EUR.");
                }

                return rateToken.Value<decimal>();
            }
        }
    }
}
