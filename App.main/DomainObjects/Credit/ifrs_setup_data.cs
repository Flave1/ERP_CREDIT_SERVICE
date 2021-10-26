using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_setup_data : GeneralEntity
    {
        [Key]
        public int SetUpId { get; set; }

        public double? Threshold { get; set; }

        public int? Deteroriation_Level { get; set; }

        public int? Classification_Type { get; set; }

        public int? Historical_PD_Year_Count { get; set; }

        public bool? PDBasis { get; set; }

        public int? Ltpdapproach { get; set; }

        public double? CCF { get; set; }

        [StringLength(50)]
        public string GroupBased { get; set; }

        public DateTime? RunDate { get; set; } 
    }
}
