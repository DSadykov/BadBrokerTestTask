using System;
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
    public class OpenExchangeRatesClient : IOpenExchangeRatesClient
    {
        private readonly string _apiKey;
        private readonly List<string> _requierdCurrencies;
        private readonly ILogger<OpenExchangeRatesClient> _logger;

        public OpenExchangeRatesClient(IConfiguration configuration, ILogger<OpenExchangeRatesClient> logger)
        {
            _apiKey = configuration["ApiKey"];
            _requierdCurrencies = configuration["RequiredCurrencies"].Split(',').Select(x => x.ToLower()).ToList();
            _logger = logger;
        }
        /// <summary>
        /// Gets <c>CurrencyRateModel</c> from openexchangerates.org/api/historical/{<c>date</c>:yyyy-MM-dd}.json?app_id={<c>_apiKey</c>}
        /// 
        /// </summary>
        /// 
        /// <returns>Returns null if an error occurred, otherwise returns <c>CurrencyRateModel</c> with rates specified in appsettings</returns>
        public async Task<CurrencyRateModel> GetRatesForADateAsync(DateTime date)
        {
            var baseUrl = $"https://openexchangerates.org/api/historical/{date:yyyy-MM-dd}.json?app_id={_apiKey}";
            RestClient client = new(baseUrl);
            try
            {
                RestResponse<CurrencyRateModel> result = await client.ExecuteAsync<CurrencyRateModel>(new RestRequest() { Method = Method.Get });
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    result.Data.DateTime = date;
                    //Selects only required currencies, which are specified in the appsettings
                    result.Data.Rates = result.Data.Rates.Where(x => _requierdCurrencies.Contains(x.Key.ToLower()))
                                               .ToDictionary(x => x.Key.ToLower(), y => y.Value);
                    _logger.LogInformation($"Successfully got currency rates from{baseUrl}");
                    _logger.LogTrace(JsonSerializer.Serialize(result.Data));
                    return result.Data;
                }
                else
                {
                    _logger.LogError($"{result.StatusCode} : {result.Content}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}