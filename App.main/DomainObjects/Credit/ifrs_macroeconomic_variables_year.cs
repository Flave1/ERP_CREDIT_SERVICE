using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_macroeconomic_variables_year : GeneralEntity
    {
        [Key]
        public int MacroEconomicVariableId { get; set; }

        public int? Year { get; set; }

        public double? GDP { get; set; }

        public double? Unemployement { get; set; }

        public double? Inflation { get; set; }

        public double? erosion { get; set; }

        public double? ForegnEx { get; set; }

        public double? Others { get; set; }

        public double? otherfactor { get; set; } 
    }
}
