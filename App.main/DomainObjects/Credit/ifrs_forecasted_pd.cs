using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_forecasted_pd : GeneralEntity
    {
        [Key]
        public int ForeCastedId { get; set; }

        public int? Year { get; set; }

        public decimal? PD1 { get; set; }

        public decimal? PD2 { get; set; }

        public decimal? PD3 { get; set; }

        public decimal? PD4 { get; set; }

        public decimal? PD5 { get; set; }

        public decimal? PD6 { get; set; }

        public decimal? PD7 { get; set; }

        public decimal? LifeTimePD { get; set; }

        [StringLength(50)]
        public string PDType { get; set; }

        [StringLength(50)]
        public string Stage { get; set; }
        public string Scenario { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Likelihood { get; set; }

        public decimal? ApplicablePD { get; set; }

        [StringLength(50)]
        public string ProductCode { get; set; }

        [StringLength(50)]
        public string LoanReferenceNumber { get; set; }

        [StringLength(50)]
        public string CompanyCode { get; set; }

        public DateTime? RunDate { get; set; } 
    }
}
