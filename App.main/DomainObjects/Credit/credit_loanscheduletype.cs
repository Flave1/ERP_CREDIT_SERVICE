using Banking.Contracts.GeneralExtension;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Banking.DomainObjects.Credit
{
    public class credit_loanscheduletype : GeneralEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LoanScheduleTypeId { get; set; }

        [Required]
        [StringLength(250)]
        public string LoanScheduleTypeName { get; set; }

        public int LoanScheduleCategoryId { get; set; }

        public virtual credit_loanschedulecategory credit_loanschedulecategory { get; set; }
    }
}
