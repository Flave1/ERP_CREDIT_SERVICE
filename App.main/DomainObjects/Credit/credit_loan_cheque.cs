using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loan_cheque : GeneralEntity
    {
        [Key]
        public int LoanChequeId { get; set; }
        public int LoanId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string ChequeNo { get; set; }
        public byte[] GeneralUpload { get; set; } 
    }
}
