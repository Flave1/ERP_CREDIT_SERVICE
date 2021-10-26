using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanscheduleperiodic : GeneralEntity
    {
        [Key]
        public int LoanSchedulePeriodicId { get; set; }

        public int LoanId { get; set; }

        public int PaymentNumber { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal StartPrincipalAmount { get; set; }

        public decimal PeriodPaymentAmount { get; set; }
        public decimal PeriodInterestAmount { get; set; }
        public decimal PeriodPrincipalAmount { get; set; }

        public decimal ClosingBalance { get; set; }

        public decimal EndPrincipalAmount { get; set; }

        public double InterestRate { get; set; }

        public decimal AmortisedStartPrincipalAmount { get; set; }

        public decimal AmortisedPeriodPaymentAmount { get; set; }

        public decimal AmortisedPeriodInterestAmount { get; set; }

        public decimal AmortisedPeriodPrincipalAmount { get; set; }

        public decimal AmortisedClosingBalance { get; set; }

        public decimal AmortisedEndPrincipalAmount { get; set; }

        public double EffectiveInterestRate { get; set; }

        public decimal? ActualRepayment { get; set; }

        public decimal? ActualRepaymentInterest { get; set; }

        public decimal? PaymentPending { get; set; }

        public decimal? PaymentPendingInterest { get; set; }
        public bool? ConfirmedPayment { get; set; }
        public string FlutterwaveRef { get; set; }

        public int? StaffId { get; set; } 
    }
}
