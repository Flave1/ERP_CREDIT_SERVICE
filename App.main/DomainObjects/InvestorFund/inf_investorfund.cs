using Banking.Contracts.GeneralExtension;
using Banking.DomainObjects.Credit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public partial class inf_investorfund : GeneralEntity
    {
        [Key]
        public int InvestorFundId { get; set; }

        public int InvestorFundCustomerId { get; set; }

        public int? ProductId { get; set; }

        public decimal? ProposedTenor { get; set; }

        public decimal? ProposedRate { get; set; }

        public int? FrequencyId { get; set; }

        [StringLength(50)]
        public string Period { get; set; }

        public decimal? ProposedAmount { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        [StringLength(50)]
        public string InvestmentPurpose { get; set; }
        public string StaffEmail { get; set; }

        public bool? EnableRollOver { get; set; }
        public bool? ConfirmedPayment { get; set; }
        public string FlutterwaveRef { get; set; }
        public bool? IsUploaded { get; set; }

        public int? InstrumentId { get; set; }

        [StringLength(50)]
        public string InstrumentNumer { get; set; }

        public DateTime? InstrumentDate { get; set; }

        public int? CustomerNameId { get; set; }

        [StringLength(50)]
        public string ProductName { get; set; }

        public decimal? ApprovedTenor { get; set; }

        public decimal? ApprovedRate { get; set; }

        public int? ApprovedProductId { get; set; }

        public DateTime? FirstPrincipalDate { get; set; }

        public DateTime? MaturityDate { get; set; }

        public decimal? ApprovedAmount { get; set; }

        public decimal? ExpectedPayout { get; set; }

        public decimal? ExpectedInterest { get; set; }

        public int? ApprovalStatus { get; set; }

        public int? InvestmentStatus { get; set; }

        public bool? GenerateCertificate { get; set; }

        [StringLength(10)]
        public string RefNumber { get; set; } 

        public string WorkflowToken { get; set; }
        public virtual credit_loancustomer credit_loancustomer { get; set; }
    }
}
