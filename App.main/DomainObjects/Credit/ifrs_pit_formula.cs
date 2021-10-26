using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_pit_formula : GeneralEntity
    {
        [Key]
        public int PitFormularId { get; set; }

        [StringLength(50)]
        public string LoanReferenceNumber { get; set; }

        public string Equation { get; set; }

        public double? ComputedPd { get; set; }

        public int? Type { get; set; }

        public DateTime? Rundate { get; set; }
    }
}
