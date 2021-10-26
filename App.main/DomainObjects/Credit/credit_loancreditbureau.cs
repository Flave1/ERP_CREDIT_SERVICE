using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loancreditbureau : GeneralEntity
    {
        [Key]
        public int LoanCreditBureauId { get; set; }

        public int LoanApplicationId { get; set; }

        public int CreditBureauId { get; set; }

        public decimal? ChargeAmount { get; set; }

        public bool? ReportStatus { get; set; }

        public byte[] SupportDocument { get; set; } 

        public virtual credit_creditbureau credit_creditbureau { get; set; }

        public virtual credit_loanapplication credit_loanapplication { get; set; }
    }
}
