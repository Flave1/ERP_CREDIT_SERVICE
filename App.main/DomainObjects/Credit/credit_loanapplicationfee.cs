using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanapplicationfee : GeneralEntity
    {
        [Key]
        public int LoanApplicationFeeId { get; set; }

        public int FeeId { get; set; }

        public int ProductPaymentType { get; set; }

        public int ProductFeeType { get; set; }

        public decimal ProductAmount { get; set; }

        public int ProductId { get; set; }
        public int LoanApplicationId { get; set; }

    }
}
