using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public partial class inf_product : GeneralEntity
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(50)]
        public string ProductCode { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        public decimal? Rate { get; set; }

        public int? ProductTypeId { get; set; }

        public int? ProductLimit { get; set; }

        public decimal? InterestRateMax { get; set; }

        public int? InterestRepaymentTypeId { get; set; }

        public int? ScheduleMethodId { get; set; }

        public int? FrequencyId { get; set; }

        public decimal? MaximumPeriod { get; set; }

        public decimal? InterestRateAnnual { get; set; }

        public decimal? InterestRateFrequency { get; set; }

        public int? ProductPrincipalGl { get; set; }

        public int? ReceiverPrincipalGl { get; set; }

        public int? InterstExpenseGl { get; set; }

        public int? InterestPayableGl { get; set; }

        public int? ProductLimitId { get; set; }

        public decimal? EarlyTerminationCharge { get; set; } 
        public decimal? TaxRate { get; set; }
        public int? TaxGl { get; set; }
    }
}
