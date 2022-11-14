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
        public async Task<List<CurrencyRateModel>> GetCurrencyRates(DateTime from, DateTime to)
        {
            List<CurrencyRateModel> cachedRates = await _dbRepository.GetRates(from, to);
            if (!cachedRates.Any())
            {
                List<CurrencyRateModel> apiRates = await GetMissedAndCache(from, to);
                return apiRates;
            }
            return await GetFromCache(from, to, cachedRates);
        }

        private async Task<List<CurrencyRateModel>> GetFromCache(DateTime from, DateTime to, List<CurrencyRateModel> cachedRates)
        {
            DateTime maxDate = cachedRates.Max(x => x.DateTime);
            DateTime minDate = cachedRates.Min(x => x.DateTime);
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
            List<CurrencyRateModel> apiRates = await _exchangeRatesLoader.GetCurrencyRates(from, to);
            await _dbRepository.AddRates(apiRates);
            return apiRates;
        }
    }
}
