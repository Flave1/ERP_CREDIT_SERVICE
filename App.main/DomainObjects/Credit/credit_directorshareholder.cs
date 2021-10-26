using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_directorshareholder : GeneralEntity
    {

        [Key]
        public int DirectorShareHolderId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        [StringLength(250)]
        public string CompanyName { get; set; }

        public double PercentageHolder { get; set; }

        public virtual credit_loancustomer credit_loancustomer { get; set; }
    }
}
