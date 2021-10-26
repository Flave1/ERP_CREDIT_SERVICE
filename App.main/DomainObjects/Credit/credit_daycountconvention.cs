using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_daycountconvention : GeneralEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DayCountConventionId { get; set; }

        [Required]
        [StringLength(250)]
        public string DayCountConventionName { get; set; }

        public int DaysInAYear { get; set; }

        public bool? IsVisible { get; set; } 
    }
}
