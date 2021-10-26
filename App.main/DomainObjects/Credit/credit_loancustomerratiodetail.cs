using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loancustomerratiodetail : GeneralEntity
    {
        [Key]
        public int RatioDetailId { get; set; }

        [StringLength(50)]
        public string RatioName { get; set; }

        [StringLength(256)]
        public string Description { get; set; }
         
    }
}
