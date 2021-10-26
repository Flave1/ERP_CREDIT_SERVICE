using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanreviewoperationirregularinput : GeneralEntity
    {
        [Key]
        public int IrregularScheduleInputId { get; set; }

        public int LoanReviewOperationId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; } 

        public virtual credit_loanreviewoperation credit_loanreviewoperation { get; set; }
    }
}
