using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanstaging : GeneralEntity

    {
        [Key]
        public int LoanStagingId { get; set; }

        public int ProbationPeriod { get; set; }

        public int From { get; set; }

        public int To { get; set; } 
    }
}
