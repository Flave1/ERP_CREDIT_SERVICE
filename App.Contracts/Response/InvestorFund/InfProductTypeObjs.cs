using GOSLibraries.GOS_API_Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Banking.Contracts.Response.InvestorFund
{
    public class InfProductTypeObj
    {
        public int ProductTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool? Active { get; set; }

        public bool? Deleted { get; set; }

        public string UpdatedBy { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? CreatedOn { get; set; }
    }

    public class AddUpdateInfProductTypeObj
    {
        public int ProductTypeId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class InfProductTypeRegRespObj
    {
        public int ProductTypeId { get; set; }
        public APIResponseStatus Status { get; set; }
    }

    public class InfProductTypeRespObj
    {
        public List<InfProductTypeObj> InfProductTypes { get; set; }
        public byte[] export { get; set; }
        public APIResponseStatus Status { get; set; }
    }
}
