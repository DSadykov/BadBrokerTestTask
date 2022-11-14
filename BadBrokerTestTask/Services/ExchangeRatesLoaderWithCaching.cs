using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;

namespace BadBrokerTestTask.Services
{
    public class ExchangeRatesLoaderWithCaching : IExchangeRatesLoader
    {
        private readonly IExchangeRatesLoader _exchangeRatesLoader;
        private readonly IDbRepository _dbRepository;

        public ExchangeRatesLoaderWithCaching(ExchangeRatesLoader exchangeRatesLoader, IDbRepository dbRepository)
        {
            _exchangeRatesLoader = exchangeRatesLoader;
            _dbRepository = dbRepository;
        }
        public async Task<List<CurrencyRateModel>> GetCurrencyRatesAsync(DateTime from, DateTime to)
        {
            List<CurrencyRateModel> cachedRates = await _dbRepository.GetRatesAsync(from, to);
            
            if (!cachedRates.Any())
            {
                //None cached data were found, downloading entire interval
                List<CurrencyRateModel> apiRates = await GetMissedAndCache(from, to);
                return apiRates;
            }
            return await GetFromCacheAsync(from, to, cachedRates);
        }

        private async Task<List<CurrencyRateModel>> GetFromCacheAsync(DateTime from, DateTime to, List<CurrencyRateModel> cachedRates)
        {
            DateTime maxDate = cachedRates.Max(x => x.DateTime);
            DateTime minDate = cachedRates.Min(x => x.DateTime);
            /*
             handles 4 situations:
                1) cached data is in the middle of dates
                2) cached data is enough from the beginning to a certain date
                3) cached data is enough from a certain date until the end
                4) cached data is enough
             */
            if (minDate > from && maxDate < to)
            {
                List<CurrencyRateModel> missedRatesMin = await GetMissedAndCache(from, minDate.AddDays(-1));
                List<CurrencyRateModel> missedRatesMax = await GetMissedAndCache(maxDate.AddDays(1), to);
                return missedRatesMin.Concat(cachedRates).Concat(missedRatesMax).ToList();
            }
            else if (minDate == from && maxDate < to)
            {
                List<CurrencyRateModel> missedRates = await GetMissedAndCache(maxDate.AddDays(1), to);
                return cachedRates.Concat(missedRates).ToList();
            }
            else if (minDate > from && maxDate == to)
            {
                List<CurrencyRateModel> missedRates = await GetMissedAndCache(from, minDate.AddDays(-1));
                return missedRates.Concat(cachedRates).ToList();
            }
            else
            {
                return cachedRates;
            }
        }

        private async Task<List<CurrencyRateModel>> GetMissedAndCache(DateTime from, DateTime to)
        {
            List<CurrencyRateModel> apiRates = await _exchangeRatesLoader.GetCurrencyRatesAsync(from, to);
            await _dbRepository.AddRatesAsync(apiRates);
            return apiRates;
        }
    }
}
