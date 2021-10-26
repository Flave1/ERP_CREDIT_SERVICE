using Banking.Contracts.GeneralExtension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.DomainObjects.Credit
{
    public class ifrs_scenario_setup : GeneralEntity
    {
        [Key]
        public int ScenarioId { get; set; }

        [StringLength(50)]
        public string Scenario { get; set; }

        public decimal? Likelihood { get; set; }

        public decimal? Rate { get; set; } 
    }
}
