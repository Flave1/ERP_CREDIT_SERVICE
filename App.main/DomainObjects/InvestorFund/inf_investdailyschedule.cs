using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public partial class inf_investdailyschedule : GeneralEntity
    {
        [Key]
        public int InvestmentDailyScheduleId { get; set; }

        public int? Period { get; set; }

        public decimal? OB { get; set; }

        public decimal? InterestAmount { get; set; }

        public decimal? CB { get; set; }

        public decimal? Repayment { get; set; }

        public DateTime? PeriodDate { get; set; }

        public int? InvestorFundId { get; set; }

        public int? PeriodId { get; set; }

        public bool? EndPeriod { get; set; } 
    }
}
