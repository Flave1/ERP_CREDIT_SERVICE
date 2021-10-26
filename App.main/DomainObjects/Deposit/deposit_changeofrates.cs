namespace GODP.Entities.Models
{
    using Banking.Contracts.GeneralExtension;
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class deposit_changeofrates : GeneralEntity
    {
        [Key]
        public int ChangeOfRateId { get; set; }

        public int? Structure { get; set; }

        public int? Product { get; set; }

        public decimal? CurrentRate { get; set; }

        public decimal? ProposedRate { get; set; }

        [StringLength(500)]
        public string Reasons { get; set; }

        [StringLength(50)]
        public string ApproverName { get; set; }

        [StringLength(50)]
        public string ApproverComment { get; set; } 
    }
}
