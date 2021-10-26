﻿using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.InvestorFund
{
    public partial class inf_liquidation : GeneralEntity
    {
        [Key]
        public int LiquidationId { get; set; }

        public int InvestorFundId { get; set; }

        public int? InvestorFundCustomerId { get; set; }

        public int? ProductId { get; set; }

        public int? ProposedTenor { get; set; }

        public decimal? ProposedRate { get; set; }

        public int? FrequencyId { get; set; }

        public int? Period { get; set; }

        public decimal? ProposedAmount { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime? EffectiveDate { get; set; }

        [StringLength(50)]
        public string InvestmentPurpose { get; set; }

        public DateTime? LiquidationDate { get; set; }

        public decimal? EarlyTerminationCharge { get; set; }

        public decimal? AmountPayable { get; set; }

        public int? DrProductPrincipal { get; set; }

        public int? CrIntExpense { get; set; }

        public int? DrIntPayable { get; set; }

        public int? CrReceiverPrincipalGL { get; set; }

        public int? ApprovalStatus { get; set; }

        [StringLength(500)]
        public string PaymentAccount { get; set; }

        [StringLength(500)]
        public string Account { get; set; } 
        public string WorkflowToken { get; set; }

    }
}
