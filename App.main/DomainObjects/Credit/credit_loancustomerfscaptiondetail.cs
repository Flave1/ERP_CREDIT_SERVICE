using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loancustomerfscaptiondetail : GeneralEntity
    {
        [Key]
        public int FSDetailId { get; set; }

        public int CustomerId { get; set; }

        public int FSCaptionId { get; set; }
        public DateTime FSDate { get; set; }
        public decimal Amount { get; set; }
         

        public virtual credit_loancustomer credit_loancustomer { get; set; }

        public virtual credit_loancustomerfscaption credit_loancustomerfscaption { get; set; }
    }
}
