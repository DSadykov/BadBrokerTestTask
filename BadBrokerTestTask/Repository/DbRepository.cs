using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;
using BadBrokerTestTask.Models.Responses;

using Dapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BadBrokerTestTask.Repository
{
    public class DbRepository : IDbRepository
    {
        private readonly RatesDbContext _ratesDbContext;
        private readonly ILogger<DbRepository> _logger;

        public DbRepository(RatesDbContext ratesDbContext, ILogger<DbRepository> logger)
        {
            _ratesDbContext = ratesDbContext;
            _logger = logger;
        }

        public async Task<List<CurrencyRateModel>> GetRatesAsync(DateTime from, DateTime to)
        {
            List<CurrencyRateModel> result = new();
            foreach (IGrouping<DateTime, CurrencyRateDbModel> groupedCurrencyRateDbModel in _ratesDbContext.CurrencyRateModels.Where(x=>x.Date>=from&&x.Date<=to).AsEnumerable().GroupBy(x => x.Date))
            {
                CurrencyRateModel currencyRateModel = new()
                {
                    DateTime = groupedCurrencyRateDbModel.Key,
                    Rates = new()
                };
                foreach (CurrencyRateDbModel currencyRateDbModel in groupedCurrencyRateDbModel)
                {
                    currencyRateModel.Rates[currencyRateDbModel.Currency] = currencyRateDbModel.Rate;
                }
                result.Add(currencyRateModel);
            }
            _logger.LogInformation($"Successfully got rates from database");
            _logger.LogTrace($"{JsonSerializer.Serialize(result)}");
            return result;
        }

        public async Task AddRatesAsync(List<CurrencyRateModel> currencyRateModels)
        {
            foreach (CurrencyRateModel currencyRateModel in currencyRateModels)
            {
                IEnumerable<CurrencyRateDbModel> currencyRateDbModels = currencyRateModel.Rates.Select(rate => new CurrencyRateDbModel()
                {
                    Currency = rate.Key,
                    Date = currencyRateModel.DateTime,
                    Rate = rate.Value
                });
                await _ratesDbContext.AddRateModels(currencyRateDbModels);
                foreach (var rate in currencyRateDbModels)
                {
                    _logger.LogInformation($"Successfully added rate {currencyRateModel.DateTime} - {rate.Currency} - {rate.Rate} to database");
                }
            }
        }

    }
}