using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_directortype : GeneralEntity
    {
        [Key]
        public int DirectorTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } 
        public virtual ICollection<credit_loancustomerdirector> credit_loancustomerdirector { get; set; }
    }
}
