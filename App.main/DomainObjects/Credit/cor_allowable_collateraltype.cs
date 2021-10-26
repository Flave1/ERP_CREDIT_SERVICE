using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class cor_allowable_collateraltype : GeneralEntity
    {
        [Key]
        public int AllowableCollateralId { get; set; }

        public int ProductId { get; set; }

        public int CollateralTypeId { get; set; }
    }
}
