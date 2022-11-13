using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using RestSharp;

namespace BadBrokerTestTask.Services
{
    public class ExchangeRatesLoader : IExchangeRatesLoader
    {

        private readonly ILogger<ExchangeRatesLoader> _logger;
        private readonly IOpenExchangeRatesClient _openExchangeRatesClient;

        public ExchangeRatesLoader(ILogger<ExchangeRatesLoader> logger, IOpenExchangeRatesClient openExchangeRatesClient)
        {

            _logger = logger;
            _openExchangeRatesClient = openExchangeRatesClient;
        }
        public async Task<List<CurrencyRateModel>> GetCurrencyRates(DateTime from, DateTime to)
        {
            var result = new List<CurrencyRateModel>();
            for (var dt = from; dt <= to; dt = dt.AddDays(1))
            {
                CurrencyRateModel item = await _openExchangeRatesClient.GetRatesForADate(dt);
                if (item is not null)
                {
                    result.Add(item);
                }
                else
                {
                    _logger.LogWarning($"Skipped rates for date{dt}");
                }
            }
            _logger.LogInformation($"Successfuly got currency rates for dates {from} - {to}");
            _logger.LogDebug(JsonSerializer.Serialize(result));
            return result;
        }
    }
}
