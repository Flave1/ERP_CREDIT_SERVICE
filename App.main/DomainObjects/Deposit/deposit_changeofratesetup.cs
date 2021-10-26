namespace GODP.Entities.Models
{
    using Banking.Contracts.GeneralExtension;
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class deposit_changeofratesetup : GeneralEntity
    {
        [Key]
        public int ChangeOfRateSetupId { get; set; }

        public int? Structure { get; set; }

        public int? ProductId { get; set; }

        public bool? CanApply { get; set; } 
    }
}
