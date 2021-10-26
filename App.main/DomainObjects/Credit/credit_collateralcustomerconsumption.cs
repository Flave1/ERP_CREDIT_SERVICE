using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_collateralcustomerconsumption : GeneralEntity
    {
        [Key]
        public int CollateralCustomerConsumptionId { get; set; }

        public int CollateralCustomerId { get; set; }

        public int LoanApplicationId { get; set; }

        public int? CustomerId { get; set; }

        public decimal CollateralCurrentAmount { get; set; }

        public decimal ActualCollateralValue { get; set; }

        public decimal Amount { get; set; }

        [StringLength(50)]
        public string CollateralType { get; set; }

        public int ExpectedCollateralValue { get; set; }

        [StringLength(50)]
        public string CollateralCode { get; set; } 
    }
}
