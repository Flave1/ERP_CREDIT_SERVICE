using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_creditriskcategory : GeneralEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CreditRiskCategoryId { get; set; }

        [Required]
        [StringLength(255)]
        public string CreditRiskCategoryName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public bool UseInOrigination { get; set; } 
    }
}
