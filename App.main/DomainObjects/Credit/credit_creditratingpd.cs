using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_creditratingpd : GeneralEntity
    {
        [Key]
        public int CreditRiskRatingPDId { get; set; }

        public decimal PD { get; set; }

        public decimal MinRangeScore { get; set; }

        public decimal MaxRangeScore { get; set; }

        public int? ProductId { get; set; }

        public decimal? InterestRate { get; set; }

        [StringLength(1000)]
        public string Description { get; set; } 
    }
}
