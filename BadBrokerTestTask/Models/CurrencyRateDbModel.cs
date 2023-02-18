using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BadBrokerTestTask.Models
{
    [Table("Rates")]
    public class CurrencyRateDbModel
    {
        [Key]
        public int Id { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }
}
