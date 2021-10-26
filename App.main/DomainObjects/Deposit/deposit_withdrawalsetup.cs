namespace GODP.Entities.Models
{
    using Banking.Contracts.GeneralExtension;
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class deposit_withdrawalsetup : GeneralEntity
    {
        [Key]
        public int WithdrawalSetupId { get; set; }

        public int? Structure { get; set; }

        public int? Product { get; set; }

        public bool? PresetChart { get; set; }

        public int? AccountType { get; set; }

        public decimal? DailyWithdrawalLimit { get; set; }

        public bool? WithdrawalCharges { get; set; }

        [StringLength(50)]
        public string Charge { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(50)]
        public string ChargeType { get; set; } 
    }
}
