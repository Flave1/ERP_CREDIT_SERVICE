using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_creditriskscorecard : GeneralEntity
    {
        [Key]
        public int CreditRiskScoreCardId { get; set; }

        public int CreditRiskAttributeId { get; set; }

        public int CustomerTypeId { get; set; }

        [Required]
        [StringLength(250)]
        public string Value { get; set; }

        public decimal Score { get; set; } 
    }
}
