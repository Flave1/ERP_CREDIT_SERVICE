using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_producttype : GeneralEntity
    {
        [Key]
        public int ProductTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductTypeName { get; set; }
        public string ProductName { get; set; } 
    }
}
