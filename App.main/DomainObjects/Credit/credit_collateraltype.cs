using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_collateraltype : GeneralEntity
    {

        [Key]
        public int CollateralTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Details { get; set; }

        public bool? RequireInsurancePolicy { get; set; }

        public int? ValuationCycle { get; set; }

        public int? HairCut { get; set; }

        public bool? AllowSharing { get; set; }

        public virtual ICollection<credit_collateralcustomer> credit_collateralcustomer { get; set; }
    }
}
