using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using BadBrokerTestTask.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BadBrokerTestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatesController : ControllerBase
    {

        private readonly ILogger<RatesController> _logger;
        private readonly IExchangeRatesLoader _exchangeRatesLoader;
        private readonly IRevenueCalculator _revenueCalculator;

        public RatesController(ILogger<RatesController> logger, IExchangeRatesLoader exchangeRatesLoader, IRevenueCalculator revenueCalculator)
        {
            _logger = logger;
            _exchangeRatesLoader = exchangeRatesLoader;
            _revenueCalculator = revenueCalculator;
        }

        [HttpGet("Best")]
        public async Task<IActionResult> Best(DateTime startDate, DateTime endDate, decimal moneyUsd)
        {
            var currencies = await _exchangeRatesLoader.GetCurrencyRates(startDate, endDate);
            var result = _revenueCalculator.CalculateHighestRevenue(currencies, moneyUsd);
            return Ok(result);
        }
    }
}
