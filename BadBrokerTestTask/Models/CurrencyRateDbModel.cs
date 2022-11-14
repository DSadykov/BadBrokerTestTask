using System;

namespace BadBrokerTestTask.Models
{
    public class CurrencyRateDbModel
    {
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }
}
