using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_fee : GeneralEntity
    {
        [Key]
        public int FeeId { get; set; }

        [Required]
        [StringLength(250)]
        public string FeeName { get; set; }

        public bool IsIntegral { get; set; }
        public bool PassEntryAtDisbursment { get; set; }
        public int? TotalFeeGL { get; set; }

        public virtual ICollection<credit_productfee> credit_productfee { get; set; }
    }
}
