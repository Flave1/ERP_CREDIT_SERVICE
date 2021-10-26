using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_loancustomerdocument : GeneralEntity
    {
        [Key]
        public int CustomerDocumentId { get; set; }

        public int DocumentTypeId { get; set; }

        public int CustomerId { get; set; }

        [StringLength(500)]
        public string PhysicalLocation { get; set; }

        [Required]
        [StringLength(255)]
        public string DocumentName { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentExtension { get; set; }

        [Required]
        public byte[] DocumentFile { get; set; } 

        public virtual credit_documenttype credit_documenttype { get; set; }

        public virtual credit_loancustomer credit_loancustomer { get; set; }
    }
}
