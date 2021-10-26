using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanapplicationrecommendationlog : GeneralEntity
    {
        [Key]
        public int LoanApplicationRecommendationLogId { get; set; }

        public int LoanApplicationId { get; set; }

        public int ApprovedProductId { get; set; }

        public int ApprovedTenor { get; set; }
        public string StaffName { get; set; }

        public double ApprovedRate { get; set; }

        public decimal ApprovedAmount { get; set; }
         

        public virtual credit_loanapplication credit_loanapplication { get; set; }

        public virtual credit_product credit_product { get; set; }
    }
}
