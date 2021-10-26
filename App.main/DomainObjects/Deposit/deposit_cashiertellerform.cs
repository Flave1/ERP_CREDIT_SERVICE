namespace GODP.Entities.Models
{
    using Banking.Contracts.GeneralExtension;
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class deposit_cashiertellerform : GeneralEntity
    {
        [Key]
        public int DepositCashierTellerId { get; set; }

        public int Structure { get; set; }

        public int? SubStructure { get; set; }

        public int? Currency { get; set; }

        public DateTime? Date { get; set; }

        public decimal? OpeningBalance { get; set; }

        public decimal? ClosingBalance { get; set; } 
    }
}
