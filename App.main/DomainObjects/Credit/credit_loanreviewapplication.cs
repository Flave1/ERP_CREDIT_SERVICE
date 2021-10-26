using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanreviewapplication : GeneralEntity
    {
        [Key]
        public int LoanReviewApplicationId { get; set; }

        public int LoanId { get; set; }

        public int ProductId { get; set; }

        public int OperationId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        public string ReviewDetails { get; set; }

        public int ApprovalStatusId { get; set; }

        public bool GenerateOfferLetter { get; set; }

        public bool? OperationPerformed { get; set; }

        public int ProposedTenor { get; set; }

        public double ProposedRate { get; set; }
        public decimal ProposedAmount { get; set; }

        public int ApprovedTenor { get; set; }

        public double ApprovedRate { get; set; }

        public decimal ApprovedAmount { get; set; }

        public int? PrincipalFrequencyTypeId { get; set; }

        public int? InterestFrequencyTypeId { get; set; }

        public DateTime? FirstPrincipalPaymentDate { get; set; }
        public DateTime? FirstInterestPaymentDate { get; set; }

        public decimal? Prepayment { get; set; }
        public string WorkflowToken { get; set; } 
    }
}
