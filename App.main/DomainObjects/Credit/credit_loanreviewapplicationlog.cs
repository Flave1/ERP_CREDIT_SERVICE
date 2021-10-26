using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanreviewapplicationlog : GeneralEntity
    {
        [Key]
        public int LoanReviewApplicationLogId { get; set; }

        public int LoanReviewApplicationId { get; set; }

        public int LoanId { get; set; }

        public int ApprovedTenor { get; set; }

        public double ApprovedRate { get; set; }
        public string StaffName { get; set; }
        public decimal ApprovedAmount { get; set; }

        public int? PrincipalFrequencyTypeId { get; set; }

        public int? InterestFrequencyTypeId { get; set; }
        public DateTime? FirstPrincipalPaymentDate { get; set; }
        public DateTime? FirstInterestPaymentDate { get; set; } 
    }
}
