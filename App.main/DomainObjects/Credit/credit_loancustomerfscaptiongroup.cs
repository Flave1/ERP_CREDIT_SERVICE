using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loancustomerfscaptiongroup : GeneralEntity
    {
        [Key]
        public int FSCaptionGroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string FSCaptionGroupName { get; set; } 
        public virtual ICollection<credit_loancustomerfscaption> credit_loancustomerfscaption { get; set; }
    }
}
