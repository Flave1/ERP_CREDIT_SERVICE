using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_frequencytype : GeneralEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FrequencyTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Mode { get; set; }

        public double Value { get; set; }

        public int? Days { get; set; }

        public bool? IsVisible { get; set; }
    }
}
