using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanapplicationcollateral : GeneralEntity
    {
        [Key]
        public int LoanApplicationCollateralId { get; set; }

        public int? CollateralCustomerId { get; set; }

        public int? LoanApplicationId { get; set; }
        public decimal ActualCollateralValue { get; set; }
         
        public virtual credit_collateralcustomer credit_collateralcustomer { get; set; }

        public virtual credit_loanapplication credit_loanapplication { get; set; }
    }
}
