using System;
using System.Collections.Generic;

namespace BadBrokerTestTask.Models
{
    public class CurrencyRateModel
    {
        public Dictionary<string, decimal> Rates {get; set; }
        public DateTime DateTime { get; set; }
    }
}
