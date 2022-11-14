using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BadBrokerTestTask.Models;

namespace BadBrokerTestTask.Interfaces
{
    public interface IDbRepository
    {
        Task AddRates(List<CurrencyRateModel> currencyRateModels);
        Task<List<CurrencyRateModel>> GetRates(DateTime from, DateTime to);
    }
}