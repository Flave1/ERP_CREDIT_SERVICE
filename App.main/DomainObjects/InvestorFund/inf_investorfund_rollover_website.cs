using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public class inf_investorfund_rollover_website : GeneralEntity
    {
        [Key]
        public int InvestorFundIdWebsiteRolloverId { get; set; }
        public int InvestorFundId { get; set; }

        public decimal? ApprovedTenor { get; set; }

        public decimal? RollOverAmount { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? ApprovalStatus { get; set; } 
    }
}
