using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BadBrokerTestTask.Models;

namespace BadBrokerTestTask.Interfaces
{
    public interface IExchangeRatesLoader
    {
        Task<List<CurrencyRateModel>> GetCurrencyRatesAsync(DateTime from, DateTime to);
    }
}