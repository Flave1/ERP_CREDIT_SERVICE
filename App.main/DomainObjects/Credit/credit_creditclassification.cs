﻿using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class credit_creditclassification : GeneralEntity
    {
        [Key]
        public int CreditClassificationId { get; set; }

        [Required]
        [StringLength(256)]
        public string Description { get; set; }

        public int ProvisioningRequirement { get; set; }

        public int? UpperLimit { get; set; }

        public int? LowerLimit { get; set; } 
    }
}
