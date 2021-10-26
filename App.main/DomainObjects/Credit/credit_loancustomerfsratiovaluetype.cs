using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loancustomerfsratiovaluetype : GeneralEntity
    {
        [Key]
        public int ValueTypeId { get; set; }

        [StringLength(50)]
        public string ValueTypeName { get; set; } 
    }
}
