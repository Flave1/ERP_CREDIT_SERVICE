using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanapplication_fee_log : GeneralEntity
    {
        [Key]
        public int LoanApplicationFeeLogId { get; set; }

        public int LoanApplicationId { get; set; }

        public int ApprovedProductId { get; set; }
        public int FeeId { get; set; }
        public int FeeTypeId { get; set; }

        public string StaffName { get; set; }

        public decimal ApprovedProductAmount { get; set; }
    }
}
