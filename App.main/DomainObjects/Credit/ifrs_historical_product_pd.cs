using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_historical_product_pd : GeneralEntity
    {
        [Key]
        public int HistoricalProductPDId { get; set; }

        public int? Year { get; set; }

        public int? Period { get; set; }

        public int? ProductId { get; set; }

        public double? PD { get; set; }

        public double? AvgPD { get; set; } 

        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
