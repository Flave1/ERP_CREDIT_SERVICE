using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanreviewofferletter : GeneralEntity
    {
        [Key]
        public int LoanreviewOfferLetterId { get; set; }

        public int LoanReviewApplicationId { get; set; }

        [StringLength(50)]
        public string ReportStatus { get; set; }

        public byte[] SupportDocument { get; set; }
    }
}
