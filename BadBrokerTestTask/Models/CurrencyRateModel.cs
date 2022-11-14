using System;
using System.Collections.Generic;

using BadBrokerTestTask.Models.Responses;

namespace BadBrokerTestTask.Models
{
    public class CurrencyRateModel
    {
        public Dictionary<string, decimal> Rates { get; set; }
        public DateTime DateTime { get; set; }
        public Rate ToRate()
        {
            Rates.TryGetValue("eur", out var eur);
            Rates.TryGetValue("gbp", out var gbp);
            Rates.TryGetValue("rub", out var rub);
            Rates.TryGetValue("jpy", out var jpy);
            return new Rate()
            {
                Eur = eur,
                Gbp = gbp,
                Rub = rub,
                Jpy = jpy,
                Date = x.DateTime
            };
        }
    }

}
