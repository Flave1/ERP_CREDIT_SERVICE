using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_customerbankdetails : GeneralEntity
    {
        [Key]
        public int CustomerBankDetailsId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        [StringLength(250)]
        public string BVN { get; set; }

        [Required]
        [StringLength(50)]
        public string Account { get; set; }

        [Required]
        [StringLength(550)]
        public string Bank { get; set; } 

        public virtual credit_loancustomer credit_loancustomer { get; set; }
    }
}
