using System.Collections.Generic;

using BadBrokerTestTask.Models;
using BadBrokerTestTask.Models.Responses;

namespace BadBrokerTestTask.Interfaces
{
    public interface IRevenueCalculator
    {
        BestRatesResponse CalculateHighestRevenue(List<CurrencyRateModel> currencies, decimal usd);
    }
}