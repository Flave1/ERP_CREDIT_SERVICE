using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_customeridentitydetails : GeneralEntity
    {
        [Key]
        public int CustomerIdentityDetailsId { get; set; }

        public int CustomerId { get; set; }

        public int IdentificationId { get; set; }

        [Required]
        [StringLength(250)]
        public string Number { get; set; }

        [Required]
        [StringLength(50)]
        public string Issuer { get; set; } 

        //public virtual cor_identification cor_identification { get; set; }

        public virtual credit_loancustomer credit_loancustomer { get; set; }
    }
}
