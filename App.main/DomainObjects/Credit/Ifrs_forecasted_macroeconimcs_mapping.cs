using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class Ifrs_forecasted_macroeconimcs_mapping : GeneralEntity
    {
        [Key]
        public int ForecastedMacroEconomicMappingId { get; set; }

        public int? Year { get; set; }

        public int? Position { get; set; }

        [StringLength(50)]
        public string LoanReferenceNumber { get; set; }

        public int? Type { get; set; }

        [StringLength(50)]
        public string Variable { get; set; }

        public double? value { get; set; }

        public DateTime? Rundate { get; set; } 

        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
