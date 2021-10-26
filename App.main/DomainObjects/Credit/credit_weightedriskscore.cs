using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_weightedriskscore : GeneralEntity
    {
        [Key]
        public int WeightedRiskScoreId { get; set; }

        public int? CreditRiskAttributeId { get; set; }

        [StringLength(50)]
        public string FeildName { get; set; }

        public bool? UseAtOrigination { get; set; }

        public int? ProductId { get; set; }

        public int? CustomerTypeId { get; set; }

        public decimal? ProductMaxWeight { get; set; }

        public decimal? WeightedScore { get; set; }  
        public virtual credit_creditriskattribute credit_creditriskattribute { get; set; }

        public virtual credit_product credit_product { get; set; }
    }
}
