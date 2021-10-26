using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public class inf_investorfund_topup_website : GeneralEntity
    {
        [Key]
        public int InvestorFundIdWebsiteTopupId { get; set; }
        public int InvestorFundId { get; set; }
        public decimal? TopUpAmount { get; set; }
        public int? ApprovalStatus { get; set; } 
    }
}
