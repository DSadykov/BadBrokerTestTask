using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BadBrokerTestTask.Models;

namespace BadBrokerTestTask.Interfaces
{
    public interface IDbRepository
    {
        Task AddRatesAsync(List<CurrencyRateModel> currencyRateModels);
        Task<List<CurrencyRateModel>> GetRatesAsync(DateTime from, DateTime to);
    }
}