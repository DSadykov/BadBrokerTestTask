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
        public double Rub { get; set; }
        public double Eur { get; set; }
        public double Gbp { get; set; }
        public double Jpy { get; set; }
    }

}
