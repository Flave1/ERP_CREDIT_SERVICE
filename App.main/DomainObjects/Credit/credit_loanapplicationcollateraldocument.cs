using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanapplicationcollateraldocument : GeneralEntity
    {
        [Key]
        public int LoanApplicationCollateralDocumentId { get; set; }

        public int? LoanApplicationId { get; set; }

        public int? CollateralTypeId { get; set; }

        public byte[] Document { get; set; }

        [StringLength(256)]
        public string DocumentName { get; set; } 

        public int? CollateralCustomerId { get; set; }
    }
}
