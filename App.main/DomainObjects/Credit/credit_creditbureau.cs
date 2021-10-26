using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_creditbureau : GeneralEntity
    {

        [Key]
        public int CreditBureauId { get; set; }

        [Required]
        [StringLength(300)]
        public string CreditBureauName { get; set; }

        public decimal CorporateChargeAmount { get; set; }

        public decimal IndividualChargeAmount { get; set; }

        public int GLAccountId { get; set; }

        public bool IsMandatory { get; set; } 

        public virtual ICollection<credit_loancreditbureau> credit_loancreditbureau { get; set; }
    }
}
