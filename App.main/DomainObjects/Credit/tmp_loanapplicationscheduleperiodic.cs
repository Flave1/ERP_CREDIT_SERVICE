using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class tmp_loanapplicationscheduleperiodic : GeneralEntity
    {
        [Key]
        public int LoanSchedulePeriodicId { get; set; }

        public int LoanApplicationId { get; set; }

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

        public int? StaffId { get; set; } 
    }
}
