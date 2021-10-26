namespace GODP.Entities.Models
{
    using Banking.Contracts.GeneralExtension;
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class deposit_cashiertellersetup : GeneralEntity
    {
        [Key]
        public int DepositCashierTellerSetupId { get; set; }

        public int? Structure { get; set; }

        public int? ProductId { get; set; }

        public bool? PresetChart { get; set; } 
    }
}
