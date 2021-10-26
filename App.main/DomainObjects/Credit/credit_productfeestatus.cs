using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_productfeestatus : GeneralEntity
    {
        [Key]
        public int ProductFeeStatusId { get; set; }

        public int LoanApplicationId { get; set; }

        public int ProductFeeId { get; set; }

        public bool Status { get; set; }

        public int? LoanId { get; set; }
    }
}
