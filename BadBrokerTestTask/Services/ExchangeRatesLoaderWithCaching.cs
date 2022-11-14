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
            var cachedRates = await _dbRepository.GetRates(from, to);
            if (!cachedRates.Any())
            {
                var apiRates = await GetMissedAndCache(from, to);
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
                var missedRatesMin = await GetMissedAndCache(from, minDate);
                var missedRatesMax = await GetMissedAndCache(maxDate, to);
                return cachedRates.Concat(missedRatesMin).Concat(missedRatesMax).ToList();
            }
            else if (minDate == from && maxDate < to)
            {
                var missedRates = await GetMissedAndCache(maxDate, to);
                return missedRates.Concat(cachedRates).ToList();
            }
            else if (minDate > from && maxDate == to)
            {
                var missedRates = await GetMissedAndCache(from, minDate);
                return missedRates.Concat(cachedRates).ToList();
            }
            else
            {
                return cachedRates;
            }
        }

        private async Task<List<CurrencyRateModel>> GetMissedAndCache(DateTime from, DateTime to)
        {
            var apiRates = await _exchangeRatesLoader.GetCurrencyRates(from, to);
            await _dbRepository.AddRates(apiRates);
            return apiRates;
        }
    }
}
