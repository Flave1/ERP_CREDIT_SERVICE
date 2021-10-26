using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loanscheduleirrigular : GeneralEntity
    {
        [Key]
        public byte LoanScheduleIrrigularId { get; set; }

        public int LoanId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; }

        public int? StaffId { get; set; } 
    }
}
