using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string _apiKey;
        private readonly List<string> _requierdCurrencies;
        private readonly ILogger<ExchangeRatesLoader> _logger;

        public ExchangeRatesLoader(IConfiguration configuration, ILogger<ExchangeRatesLoader> logger)
        {
            _apiKey = configuration["ApiKey"];
            _requierdCurrencies = configuration["RequiredCurrencies"].Split(',').Select(x => x.ToLower()).ToList();
            _logger = logger;
        }
        public async Task<List<CurrencyRateModel>> GetCurrencyRates(DateTime from, DateTime to)
        {
            var result = new List<CurrencyRateModel>();
            for (var dt = from; dt <= to; dt = dt.AddDays(1))
            {
                result.Add(await GetRatesForADate(dt));
            }
            return result;
        }
        private async Task<CurrencyRateModel> GetRatesForADate(DateTime date)
        {
            var client = new RestClient($"https://openexchangerates.org/api/historical/{date:yyyy-MM-dd}.json?app_id={_apiKey}");
            try
            {
                var result = await client.GetJsonAsync<CurrencyRateModel>("");
                result.DateTime = date;
                result.Rates = result.Rates.Where(x => _requierdCurrencies.Contains(x.Key.ToLower()))
                                           .ToDictionary(x => x.Key.ToLower(), y => y.Value);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new();
            }
        }
    }
}
