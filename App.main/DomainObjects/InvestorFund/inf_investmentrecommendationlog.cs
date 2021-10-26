using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public partial class inf_investmentrecommendationlog : GeneralEntity
    {
        [Key]
        public int InvestmentRecommendationLogId { get; set; }

        public int InvestorFundId { get; set; }

        public int ApprovedProductId { get; set; }

        public int ApprovedTenor { get; set; }

        public decimal ApprovedRate { get; set; }

        [Column(TypeName = "money")]
        public decimal ApprovedAmount { get; set; } 
    }
}
