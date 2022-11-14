using System;
using System.Threading.Tasks;

using BadBrokerTestTask.Models;

namespace BadBrokerTestTask.Interfaces
{
    public interface IOpenExchangeRatesClient
    {
        Task<CurrencyRateModel> GetRatesForADateAsync(DateTime date);
    }
}