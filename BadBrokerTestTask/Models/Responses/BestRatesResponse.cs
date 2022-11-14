using System;
using System.Collections.Generic;

namespace BadBrokerTestTask.Models.Responses
{
    public class BestRatesResponse
    {
        public decimal Revenue { get; set; }

        public DateTime Buydate { get; set; }

        public DateTime SellDate { get; set; }

        public string Tool { get; set; }

        public List<Rate> Rates { get; set; }

    }
    public class Rate
    {
        public DateTime Date { get; set; }
        public decimal Rub { get; set; }
        public decimal Eur { get; set; }
        public decimal Gbp { get; set; }
        public decimal Jpy { get; set; }
    }

}
