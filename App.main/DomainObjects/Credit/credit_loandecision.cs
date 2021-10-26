using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loandecision : GeneralEntity
    {
        [Key]
        public int LoanDecisionId { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(500)]
        public string Decision { get; set; }

        public int LoanId { get; set; }

        public int LoanScheduleId { get; set; } 
    }
}
