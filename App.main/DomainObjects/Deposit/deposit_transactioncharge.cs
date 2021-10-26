namespace GODP.Entities.Models
{
    using Banking.Contracts.GeneralExtension;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class deposit_transactioncharge : GeneralEntity
    {

        [Key]
        public int TransactionChargeId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string FixedOrPercentage { get; set; }

        public decimal? Amount_Percentage { get; set; }

        [StringLength(500)]
        public string Description { get; set; } 

        //public virtual ICollection<deposit_accountsetup> deposit_accountsetup { get; set; }

        //public virtual ICollection<deposit_selectedTransactioncharge> deposit_selectedTransactioncharge { get; set; }
    }
}
