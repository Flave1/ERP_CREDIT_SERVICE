using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public class inf_investdailyschedule_topup : GeneralEntity
    {
        [Key]
        public int InvestmentDailyScheduleTopUpId { get; set; }

        public int? Period { get; set; }

        public decimal? OB { get; set; }

        public decimal? InterestAmount { get; set; }

        public decimal? CB { get; set; }

        public decimal? Repayment { get; set; }

        public DateTime? PeriodDate { get; set; }

        public int InvestorFundId { get; set; }
        public string ReferenceNo { get; set; }

        public int? PeriodId { get; set; }

        public bool? EndPeriod { get; set; } 
    }
}
