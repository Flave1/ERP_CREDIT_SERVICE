using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_customercarddetails : GeneralEntity
    {
        [Key]
        public int CustomerCardDetailsId { get; set; }

        public int CustomerId { get; set; }

        [Required]
        [StringLength(250)]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Cvv { get; set; }

        [Required]
        [StringLength(550)]
        public string ExpiryMonth { get; set; }

        [Required]
        [StringLength(550)]
        public string ExpiryYear { get; set; }

        [Required]
        public string currencyCode { get; set; }
        public string IssuingBank { get; set; } 
    }
}
