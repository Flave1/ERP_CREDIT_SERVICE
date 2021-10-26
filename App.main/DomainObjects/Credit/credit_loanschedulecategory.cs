using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanschedulecategory : GeneralEntity
    {
        [Key]
        public int LoanScheduleCategoryId { get; set; }

        [Required]
        [StringLength(250)]
        public string LoanScheduleCategoryName { get; set; } 

        public virtual ICollection<credit_loanscheduletype> credit_loanscheduletype { get; set; }
    }
}
