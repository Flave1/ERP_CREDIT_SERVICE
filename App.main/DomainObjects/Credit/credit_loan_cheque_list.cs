using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loan_cheque_list : GeneralEntity
    {
        [Key]
        public int LoanChequeListId { get; set; }
        public int LoanId { get; set; }
        public int LoanChequeId { get; set; }
        public int Status { get; set; }
        public string ChequeNo { get; set; }
        public string StatusName { get; set; }
        public byte[] SingleUpload { get; set; }
        public decimal Amount { get; set; }
    }
}
