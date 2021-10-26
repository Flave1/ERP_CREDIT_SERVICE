using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_forecasted_lgd : GeneralEntity
    {
        [Key]
        public int ForeCastedId { get; set; }

        public int? Year { get; set; }

        public decimal? LGD1 { get; set; }

        public decimal? LGD2 { get; set; }

        public decimal? LGD3 { get; set; }

        public decimal? LGD4 { get; set; }

        public decimal? LGD5 { get; set; }

        public decimal? LGD6 { get; set; }

        public decimal? LGD7 { get; set; }

        public decimal? LifeTimeLGD { get; set; }

        [StringLength(50)]
        public string LGDType { get; set; }

        public decimal? ApplicableLGD { get; set; }

        [StringLength(50)]
        public string ProductCode { get; set; }

        [StringLength(50)]
        public string LoanReferenceNumber { get; set; }

        [StringLength(50)]
        public string CompanyCode { get; set; }

        public DateTime? RunDate { get; set; }
         
         
    }
}
