using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_productfee : GeneralEntity
    {
        [Key]
        public int ProductFeeId { get; set; }

        public int FeeId { get; set; }

        public int ProductPaymentType { get; set; }

        public int ProductFeeType { get; set; }

        public double ProductAmount { get; set; }

        public int ProductId { get; set; } 

        public virtual credit_fee credit_fee { get; set; }

        public virtual credit_product credit_product { get; set; }

        public virtual credit_repaymenttype credit_repaymenttype { get; set; }
    }
}
