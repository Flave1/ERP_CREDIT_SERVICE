namespace GODP.Entities.Models
{
    using Banking.Contracts.GeneralExtension;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class deposit_category : GeneralEntity
    { 
        public deposit_category()
        {
            deposit_accountsetup = new HashSet<deposit_accountsetup>();
        }

        [Key]
        public int CategoryId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; } 
        public virtual ICollection<deposit_accountsetup> deposit_accountsetup { get; set; }
    }
}
