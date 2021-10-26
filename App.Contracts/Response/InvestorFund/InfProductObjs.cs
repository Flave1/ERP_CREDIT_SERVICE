using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Banking.Contracts.Response.InvestorFund
{
    public class InfProductObj
    {
        public int ProductId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        public string InterestRepaymentTypeName { get; set; }
        public string ProductTypeName { get; set; }
        public string ScheduleMethodName { get; set; }
        public string FrequencyName { get; set; }

        public decimal? Rate { get; set; }

        public int? ProductTypeId { get; set; }

        public int? ProductLimit { get; set; }

        public decimal? InterestRateMax { get; set; }

        public int? InterestRepaymentTypeId { get; set; }

        public int? ScheduleMethodId { get; set; }

        public int? FrequencyId { get; set; }

        public decimal? MaximumPeriod { get; set; }

        public decimal? InterestRateAnnual { get; set; }

        public decimal? InterestRateFrequency { get; set; }

        public int? ProductPrincipalGl { get; set; }

        public int? ReceiverPrincipalGl { get; set; }

        public int? InterstExpenseGl { get; set; }

        public int? InterestPayableGl { get; set; }
        public string ProductPrincipalGLCode { get; set; }
        public string ReceiverPrincipalGlCode { get; set; }
        public string InterstExpenseGlCode { get; set; }
        public string TaxGlCode { get; set; }
        public string InterestPayableGlCode { get; set; }

        public int? ProductLimitId { get; set; }

        public decimal? EarlyTerminationCharge { get; set; }
        public decimal? TaxRate { get; set; }
        public int? TaxGl { get; set; }

        public bool? Active { get; set; }

        public bool? Deleted { get; set; }

        public string UpdatedBy { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? CreatedOn { get; set; }
    }

    public class InfProductRespObj
    {
        public List<InfProductObj> InfProducts { get; set; }
        public byte[] export { get; set; }
        public APIResponseStatus Status { get; set; }
    }


  

    //public class DeleteRequest
    //{
    //    public List<int> ItemIds { get; set; }
    //}

    //public class DeleteRespObjt
    //{
    //    public bool Deleted { get; set; }
    //    public APIResponseStatus Status { get; set; }
    //}
}
