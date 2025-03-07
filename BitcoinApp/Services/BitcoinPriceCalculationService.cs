using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinApp.Services
{
    public class BitcoinPriceCalculatorService
    {
        private readonly CoindeskService _coindeskService;
        private readonly CnbService _cnbService;

        public BitcoinPriceCalculatorService(CoindeskService coindeskService, CnbService cnbService)
        {
            _coindeskService = coindeskService;
            _cnbService = cnbService;
        }

        public async Task<(decimal bitcoinPriceEUR, decimal bitcoinPriceCZK)> GetBitcoinPriceAsync()
        {
            // Získání ceny Bitcoinu v EUR
            decimal bitcoinPriceEUR = 1;//test only  await _coindeskService.GetBitcoinPriceInEURAsync();

            // Získání kurzu EUR -> CZK
            decimal eurToCzkRate = await _cnbService.GetEurToCzkRateAsync();

            // Vypočítání ceny Bitcoinu v CZK
            decimal bitcoinPriceCZK = bitcoinPriceEUR * eurToCzkRate;

            // Vrátí dvojici (tuple) obsahující obě hodnoty
            return (bitcoinPriceEUR, bitcoinPriceCZK);
        }
    }
}
