using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_product_regressed_pd : GeneralEntity
    {
        [Key]
        public int ProductRegressedPDId { get; set; }

        [StringLength(50)]
        public string LoanReferenceNumber { get; set; }

        public int Year { get; set; }

        public double AnnualPD { get; set; }

        public double? LifeTimePD { get; set; }
         

        public DateTime? RunDate { get; set; }

        [StringLength(50)]
        public string ProductCode { get; set; } 

        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
