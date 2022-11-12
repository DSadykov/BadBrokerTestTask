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

        public RatesController(ILogger<RatesController> logger, IExchangeRatesLoader exchangeRatesLoader)
        {
            _logger = logger;
            _exchangeRatesLoader = exchangeRatesLoader;
        }

        [HttpGet("Best")]
        public async Task<string> Best(DateTime startDate, DateTime endDate, decimal moneyUsd)
        {
            return JsonSerializer.Serialize(await _exchangeRatesLoader.GetCurrencyRates(startDate, endDate));
        }
    }
}
