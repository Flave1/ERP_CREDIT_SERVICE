using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_offerletter : GeneralEntity
    {
        [Key]
        public int OfferLetterId { get; set; }

        public int LoanApplicationId { get; set; }

        [StringLength(50)]
        public string ReportStatus { get; set; }
        public string FileName { get; set; }

        public byte[] SupportDocument { get; set; } 
    }
}
