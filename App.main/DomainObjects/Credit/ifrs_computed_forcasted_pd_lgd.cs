using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_computed_forcasted_pd_lgd : GeneralEntity
    {
        [Key]
        public int ComputedPDId { get; set; }

        [StringLength(50)]
        public string ProductCode { get; set; }

        public int? Year { get; set; }

        public int? Type { get; set; }

        public double? PD_LGD { get; set; }

        public double? PD { get; set; }

        public DateTime? Rundate { get; set; } 

        [MaxLength(8)]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
