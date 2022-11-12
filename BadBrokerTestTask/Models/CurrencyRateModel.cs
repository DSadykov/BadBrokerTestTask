using System;

namespace BadBrokerTestTask.Models
{
    public class CurrencyRateModel
    {
        public string Currency { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }
    }
}
