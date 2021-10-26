using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_historical_macroeconimcs_mapping : GeneralEntity
    {
        [Key]
        public int HistoricalMacroEconomicMappingId { get; set; }

        public int? ProductId { get; set; }

        [StringLength(150)]
        public string Variable { get; set; }

        public int? Position { get; set; }

        public int? Type { get; set; }

        public double? Value { get; set; }

        public int? Year { get; set; } 

        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
