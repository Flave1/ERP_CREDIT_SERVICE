using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loancustomerfscaption : GeneralEntity
    {

        [Key]
        public int FSCaptionId { get; set; }

        [Required]
        [StringLength(1000)]
        public string FSCaptionName { get; set; }

        public int? FSCaptionGroupId { get; set; } 
        public virtual credit_loancustomerfscaptiongroup credit_loancustomerfscaptiongroup { get; set; }
        public virtual ICollection<credit_loancustomerfscaptiondetail> credit_loancustomerfscaptiondetail { get; set; }
    }
}
