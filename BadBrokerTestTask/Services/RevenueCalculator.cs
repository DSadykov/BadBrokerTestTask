using System;
using System.Collections.Generic;
using System.Linq;

using BadBrokerTestTask.Interfaces;
using BadBrokerTestTask.Models;
using BadBrokerTestTask.Models.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BadBrokerTestTask.Services
{
    public class RevenueCalculator : IRevenueCalculator
    {
        private readonly ILogger<RevenueCalculator> _logger;
        private readonly List<string> _requierdCurrencies;

        public RevenueCalculator(ILogger<RevenueCalculator> logger, IConfiguration configuration)
        {
            _logger = logger;
            _requierdCurrencies = configuration["RequiredCurrencies"].Split(',').Select(x => x.ToLower()).ToList();
        }
        public BestRatesResponse CalculateHighestRevenue(List<CurrencyRateModel> currencies, decimal usd)
        {
            var result = new BestRatesResponse()
            {
                Rates = currencies.Select(x => new Rate()
                {
                    Eur = x.Rates["eur"],
                    Gbp = x.Rates["gbp"],
                    Rub = x.Rates["rub"],
                    Jpy = x.Rates["jpy"],
                    Date = x.DateTime
                }).ToList(),
                Revenue=decimal.MinValue

            };
            foreach (var currency in _requierdCurrencies)
            {
                var counter = 1;
                foreach (var buyCurrency in currencies)
                {
                    foreach (var sellCurrency in currencies.Skip(counter))
                    {
                        var tmpRevenue = CountRevenue(buyCurrency, sellCurrency, currency, usd);
                        if (tmpRevenue > result.Revenue)
                        {
                            result.Revenue = tmpRevenue;
                            result.Buydate = buyCurrency.DateTime;
                            result.SellDate = sellCurrency.DateTime;
                            result.Tool = currency;
                        }
                    }
                    counter++;
                }
            }
            _logger.LogInformation($"Successfully calculated highest revenue, revenue is {result.Revenue}, buy at {result.Buydate}, sell at {result.SellDate}, tool is {result.Tool}");
            return result;
        }

        private decimal CountRevenue(CurrencyRateModel buyCurrency, CurrencyRateModel sellCurrency, string currency, decimal usd)
        {
            var daysNumber = 0;
            for (var dt = buyCurrency.DateTime; dt <= sellCurrency.DateTime; dt = dt.AddDays(1))
            {
                daysNumber++;
            }

            if (buyCurrency.Rates.TryGetValue(currency, out var buyRate) && sellCurrency.Rates.TryGetValue(currency, out var sellRate))
            {
                
                var revenue = usd - (buyRate * usd / sellRate) - daysNumber;
                _logger.LogDebug($"Revenue for {currency} at {buyCurrency.DateTime} is {revenue}");
                return revenue;
            }
            _logger.LogWarning($"{currency} was not found for {buyCurrency.DateTime} date!");
            return usd - daysNumber;
        }
    }
}
