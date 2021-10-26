using Banking.Contracts.Response.Credit;
using Banking.Data;
using Banking.Enum;
using Banking.Repository.Interface.Credit;
using Banking.Requests;
using Finance.Contracts.Response.Reports;
using GOSLibraries.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Banking.Contracts.Response.Credit.LoanObjs;
using static Banking.Contracts.Response.Credit.LoanScheduleObjs;
using static Banking.Contracts.Response.Credit.ProductObjs;

namespace Banking.Repository.Implement.Credit
{
    public class ReportService : IReportService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IIdentityServerRequest _serverRequest;
        public ReportService(DataContext context, IConfiguration configuration, IIdentityServerRequest serverRequest)
        {
            _context = context;
            _configuration = configuration;
            _serverRequest = serverRequest;
        }

        public List<LoanPaymentSchedulePeriodicObj> GetOfferLeterPeriodicSchedule(string applicationRefNumber)
        {
            var LoanApplicationId = (from a in _context.credit_loanapplication where a.ApplicationRefNumber == applicationRefNumber select a.LoanApplicationId).FirstOrDefault();
            var data = (from a in _context.tmp_loanapplicationscheduleperiodic
                        where a.LoanApplicationId == LoanApplicationId
                        select new LoanPaymentSchedulePeriodicObj
                        {
                            LoanId = a.LoanApplicationId,
                            PaymentNumber = a.PaymentNumber,
                            PaymentDate = a.PaymentDate.Date,
                            StartPrincipalAmount = (double)a.StartPrincipalAmount,
                            PeriodPaymentAmount = (double)a.PeriodPaymentAmount,
                            PeriodInterestAmount = (double)a.PeriodInterestAmount,
                            PeriodPrincipalAmount = (double)a.PeriodPrincipalAmount,
                            EndPrincipalAmount = (double)a.EndPrincipalAmount,
                            InterestRate = a.InterestRate,

                            AmortisedStartPrincipalAmount = (double)a.AmortisedStartPrincipalAmount,
                            AmortisedPeriodPaymentAmount = (double)a.AmortisedPeriodPaymentAmount,
                            AmortisedPeriodInterestAmount = (double)a.AmortisedPeriodInterestAmount,
                            AmortisedPeriodPrincipalAmount = (double)a.AmortisedPeriodPrincipalAmount,
                            AmortisedEndPrincipalAmount = (double)a.AmortisedEndPrincipalAmount,
                            EffectiveInterestRate = a.EffectiveInterestRate,
                        }).OrderBy(x => x.PaymentNumber).ToList();
            return data;
        }
        public List<ProductFeeObj> GetLoanApplicationFee(string applicationRefNumber)
        {
            try
            {
                var fees = (from a in _context.credit_loanapplication
                            join b in _context.credit_product on a.ApprovedProductId equals b.ProductId
                            join c in _context.credit_productfee on a.ApprovedProductId equals c.ProductId
                            join d in _context.credit_fee on c.FeeId equals d.FeeId
                            where a.ApplicationRefNumber == applicationRefNumber && a.Deleted == false &&
                                     a.ApprovalStatusId == (int)ApprovalStatus.Approved && a.LoanApplicationStatusId == (int)ApplicationStatus.OfferLetter
                            select new ProductFeeObj()
                            {
                                FeeName = d.FeeName,
                                RateValue = (decimal)c.ProductAmount,
                                ProductName = b.ProductName
                            }).ToList();

                if (fees != null)
                {
                    return fees;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new List<ProductFeeObj>();
        }
        public OfferLetterDetailObj GenerateOfferLetterLMS(string loanRefNumber)
        {
            var _Companies = _serverRequest.GetAllCompanyAsync().Result;
            var _Currencies = _serverRequest.GetCurrencyAsync().Result;
            var offerLetterDetails = (from a in _context.credit_loanreviewapplication
                                      join c in _context.credit_loan on a.LoanId equals c.LoanId
                                      join b in _context.credit_loancustomer on a.CustomerId equals b.CustomerId into cc
                                      from b in cc.DefaultIfEmpty()
                                      where c.LoanRefNumber == loanRefNumber && a.Deleted == false &&
                                      a.ApprovalStatusId == (int)ApprovalStatus.Approved
                                      select new OfferLetterDetailObj
                                      {
                                          ProductName = _context.credit_product.FirstOrDefault(x => x.ProductId == a.ProductId).ProductName,
                                          LoanApplicationId = loanRefNumber,
                                          CustomerName = _context.credit_loancustomer.FirstOrDefault(x => x.CustomerId == a.CustomerId).CustomerTypeId != 2 ? b.FirstName + " " + b.LastName : b.FirstName,
                                          Tenor = a.ApprovedTenor,
                                          InterestRate = a.ApprovedRate,
                                          LoanAmount = a.ApprovedAmount,
                                          ExchangeRate = c.ExchangeRate,
                                          CustomerAddress = b.Address ?? string.Empty,
                                          CustomerEmailAddress = b.Email,
                                          CustomerPhoneNumber = b.PhoneNo,
                                          ApplicationDate = a.CreatedOn ?? DateTime.Now,
                                          RepaymentSchedule = "Not applicable",
                                          RepaymentTerms = "Not applicable",
                                          Purpose = "",
                                          CompanyId = c.CompanyId,
                                      }).FirstOrDefault();

                offerLetterDetails.CompanyName = _Companies.companyStructures.FirstOrDefault(x => x.companyStructureId == offerLetterDetails.CompanyId)?.name;
                offerLetterDetails.CurrencyName = _Currencies.commonLookups.FirstOrDefault(x => x.LookupId == offerLetterDetails.CurrencyId)?.LookupName;
                return offerLetterDetails;
        }
        public List<LoanPaymentSchedulePeriodicObj> GetOfferLeterPeriodicScheduleLMS(string loanRefNumber)
        {
            var LoanApplicationId = (from a in _context.credit_loan where a.LoanRefNumber == loanRefNumber select a.LoanId).FirstOrDefault();
            var data = (from a in _context.credit_loanscheduleperiodic
                        where a.LoanId == LoanApplicationId && a.Deleted == false
                        select new LoanPaymentSchedulePeriodicObj
                        {
                            LoanId = a.LoanId,
                            PaymentNumber = a.PaymentNumber,
                            PaymentDate = a.PaymentDate.Date,
                            StartPrincipalAmount = (double)a.StartPrincipalAmount,
                            PeriodPaymentAmount = (double)a.PeriodPaymentAmount,
                            PeriodInterestAmount = (double)a.PeriodInterestAmount,
                            PeriodPrincipalAmount = (double)a.PeriodPrincipalAmount,
                            EndPrincipalAmount = (double)a.EndPrincipalAmount,
                            InterestRate = a.InterestRate,

                            AmortisedStartPrincipalAmount = (double)a.AmortisedStartPrincipalAmount,
                            AmortisedPeriodPaymentAmount = (double)a.AmortisedPeriodPaymentAmount,
                            AmortisedPeriodInterestAmount = (double)a.AmortisedPeriodInterestAmount,
                            AmortisedPeriodPrincipalAmount = (double)a.AmortisedPeriodPrincipalAmount,
                            AmortisedEndPrincipalAmount = (double)a.AmortisedEndPrincipalAmount,
                            EffectiveInterestRate = a.EffectiveInterestRate,
                        }).OrderBy(x=>x.PaymentNumber).ToList();
            return data;
        }


        ///Financial Statement
        public List<FSReportObj> GetFSReport(DateTime? date1, DateTime? date2)
        {
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            var fsreport = new List<FSReportObj>();
            using (var con = connection)
            {
                var cmd = new SqlCommand("fs_report_summary", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@d1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@d2",
                    Value = date2,
                });

                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var fs = new FSReportObj();

                    if (reader["GlMappingId"] != DBNull.Value)
                        fs.GlMappingId = int.Parse(reader["GlMappingId"].ToString());

                    if (reader["Caption"] != DBNull.Value)
                        fs.Caption = (reader["Caption"].ToString());

                    if (reader["SubCaption"] != DBNull.Value)
                        fs.SubCaption = (reader["SubCaption"].ToString());

                    if (reader["CompanyId"] != DBNull.Value)
                        fs.CompanyId = int.Parse(reader["CompanyId"].ToString());

                    if (reader["CompanyName"] != DBNull.Value)
                        fs.CompanyName = (reader["CompanyName"].ToString());

                    if (reader["SubPosition"] != DBNull.Value)
                        fs.SubPosition = int.Parse(reader["SubPosition"].ToString());

                    if (reader["GlCode"] != DBNull.Value)
                        fs.GlCode = (reader["GlCode"].ToString());

                    if (reader["ParentCaption"] != DBNull.Value)
                        fs.ParentCaption = (reader["ParentCaption"].ToString());

                    if (reader["Position"] != DBNull.Value)
                        fs.Position = int.Parse(reader["Position"].ToString());

                    if (reader["AccountType"] != DBNull.Value)
                        fs.AccountType = (reader["AccountType"].ToString());
                    if (reader["AccountTypeId"] != DBNull.Value)
                        fs.AccountTypeId = int.Parse(reader["AccountTypeId"].ToString());
                    if (reader["StatementType"] != DBNull.Value)
                        fs.StatementType = (reader["StatementType"].ToString());
                    if (reader["StatementTypeId"] != DBNull.Value)
                        fs.StatementTypeId = int.Parse(reader["StatementTypeId"].ToString());
                    if (reader["SubGlCode"] != DBNull.Value)
                        fs.SubGlCode = (reader["SubGlCode"].ToString());
                    if (reader["SubGlName"] != DBNull.Value)
                        fs.SubGlName = (reader["SubGlName"].ToString());
                    if (reader["GlId"] != DBNull.Value)
                        fs.GlId = int.Parse(reader["GlId"].ToString());
                    if (reader["CB"] != DBNull.Value)
                        fs.CB = decimal.Parse(reader["CB"].ToString());
                    if (reader["SubGlCode"] != DBNull.Value)
                        fs.SubGlCode1 = (reader["SubGlCode"].ToString());
                    if (reader["RunDate"] != DBNull.Value)
                        fs.RunDate = DateTime.Parse(reader["RunDate"].ToString()).Date;
                    if (reader["PreRunDate"] != DBNull.Value)
                        fs.PreRunDate = DateTime.Parse(reader["PreRunDate"].ToString()).Date;
                    fsreport.Add(fs);
                }
                con.Close();
            }
            return fsreport;
        }
        public List<FSReportObj> GetPLReport(DateTime? date1, DateTime? date2)
        {
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            var fsreport = new List<FSReportObj>();
            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("pl_report_summary", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@d1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@d2",
                    Value = date2,
                });

                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var fs = new FSReportObj();

                    if (reader["GlMappingId"] != DBNull.Value)
                        fs.GlMappingId = int.Parse(reader["GlMappingId"].ToString());

                    if (reader["Caption"] != DBNull.Value)
                        fs.Caption = (reader["Caption"].ToString());

                    if (reader["SubCaption"] != DBNull.Value)
                        fs.SubCaption = (reader["SubCaption"].ToString());

                    if (reader["CompanyId"] != DBNull.Value)
                        fs.CompanyId = int.Parse(reader["CompanyId"].ToString());

                    if (reader["CompanyName"] != DBNull.Value)
                        fs.CompanyName = (reader["CompanyName"].ToString());

                    if (reader["SubPosition"] != DBNull.Value)
                        fs.SubPosition = int.Parse(reader["SubPosition"].ToString());

                    if (reader["GlCode"] != DBNull.Value)
                        fs.GlCode = (reader["GlCode"].ToString());

                    if (reader["ParentCaption"] != DBNull.Value)
                        fs.ParentCaption = (reader["ParentCaption"].ToString());

                    if (reader["Position"] != DBNull.Value)
                        fs.Position = int.Parse(reader["Position"].ToString());

                    if (reader["AccountType"] != DBNull.Value)
                        fs.AccountType = (reader["AccountType"].ToString());
                    if (reader["AccountTypeId"] != DBNull.Value)
                        fs.AccountTypeId = int.Parse(reader["AccountTypeId"].ToString());
                    if (reader["StatementType"] != DBNull.Value)
                        fs.StatementType = (reader["StatementType"].ToString());
                    if (reader["StatementTypeId"] != DBNull.Value)
                        fs.StatementTypeId = int.Parse(reader["StatementTypeId"].ToString());
                    if (reader["SubGlCode"] != DBNull.Value)
                        fs.SubGlCode = (reader["SubGlCode"].ToString());
                    if (reader["SubGlName"] != DBNull.Value)
                        fs.SubGlName = (reader["SubGlName"].ToString());
                    if (reader["GlId"] != DBNull.Value)
                        fs.GlId = int.Parse(reader["GlId"].ToString());
                    if (reader["CB"] != DBNull.Value)
                        fs.CB = decimal.Parse(reader["CB"].ToString());
                    if (reader["SubGlCode"] != DBNull.Value)
                        fs.SubGlCode1 = (reader["SubGlCode"].ToString());
                    if (reader["RunDate"] != DBNull.Value)
                        fs.RunDate = DateTime.Parse(reader["RunDate"].ToString()).Date;
                    if (reader["PreRunDate"] != DBNull.Value)
                        fs.PreRunDate = DateTime.Parse(reader["PreRunDate"].ToString()).Date;
                    fsreport.Add(fs);
                }
                con.Close();
            }
            return fsreport;
        }




        ///Investment Reports
        public List<CorporateInvestorCustomerObj> GetInvestmentCustomerCorporate(DateTime? date1, DateTime? date2, int ct)
        {
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            var staffLists = _serverRequest.GetAllStaffAsync().Result;
            var corporateInvestorCustmer = new List<CorporateInvestorCustomerObj>();
            using (var con = connection)
            {
                var cmd = new SqlCommand("investment_customer_get_all", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date2",
                    Value = date2,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@customerType",
                    Value = ct,
                });

                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var cc = new CorporateInvestorCustomerObj();

                    if (reader["SN"] != DBNull.Value)
                        cc.SN = int.Parse(reader["SN"].ToString());

                    if (reader["CustomerID"] != DBNull.Value)
                        cc.CustomerID = int.Parse(reader["CustomerID"].ToString());

                    if (reader["DateOfIncorporation"] != DBNull.Value)
                        cc.DateOfIncorporation = DateTime.Parse(reader["DateOfIncorporation"].ToString());

                    if (reader["CompanyName"] != DBNull.Value)
                        cc.CompanyName = (reader["CompanyName"].ToString());

                    if (reader["Industry"] != DBNull.Value)
                        cc.Industry = (reader["Industry"].ToString());

                    if (reader["Size"] != DBNull.Value)
                        cc.Size = (reader["Size"].ToString());

                    if (reader["PhoneNumber"] != DBNull.Value)
                        cc.PhoneNumber = (reader["PhoneNumber"].ToString());

                    if (reader["RelationshipManagerId"] != DBNull.Value)
                        cc.RelationshipManagerId = int.Parse(reader["RelationshipManagerId"].ToString());

                    if (reader["PoliticallyExposed"] != DBNull.Value)
                        cc.PoliticallyExposed = (reader["PoliticallyExposed"].ToString());

                    if (reader["CurrentBalance"] != DBNull.Value)
                        cc.CurrentBalance = (reader["CurrentBalance"].ToString());

                    if (reader["From"] != DBNull.Value)
                        cc.From = DateTime.Parse(reader["From"].ToString());

                    if (reader["To"] != DBNull.Value)
                        cc.To = DateTime.Parse(reader["To"].ToString());
                    if (reader["Total"] != DBNull.Value)
                        cc.Total = (reader["Total"].ToString());
                    if (reader["Sum"] != DBNull.Value)
                        cc.Sum = (reader["Sum"].ToString());
                    corporateInvestorCustmer.Add(cc);
                }
                con.Close();
            }
            foreach (var item in corporateInvestorCustmer)
            {
                item.RelationshipOfficer = staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.firstName + " " + staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.lastName;
            }
            return corporateInvestorCustmer;
        }
        public List<IndividualInvestorCustomerObj> GetInvestmentCustomerIndividual(DateTime? date1, DateTime? date2, int ct)
        {
            var staffLists = _serverRequest.GetAllStaffAsync().Result;
            var genderLists = _serverRequest.GetGenderAsync().Result;
            var maritalLists = _serverRequest.GetMaritalStatusAsync().Result;
            var employmentTypeLists = _serverRequest.GetEmploymentTypeAsync().Result;
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            var investmentCustomerIndividual = new List<IndividualInvestorCustomerObj>();
            using (var con = connection)
            {
                var cmd = new SqlCommand("investment_customer_get_all", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date2",
                    Value = date2,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@customerType",
                    Value = ct,
                });

                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var cc = new IndividualInvestorCustomerObj();

                    if (reader["SN"] != DBNull.Value)
                        cc.SN = int.Parse(reader["SN"].ToString());

                    if (reader["CustomerID"] != DBNull.Value)
                        cc.CustomerID = int.Parse(reader["CustomerID"].ToString());

                    if (reader["CustomerName"] != DBNull.Value)
                        cc.CustomerName = (reader["CustomerName"].ToString());

                    if (reader["DateofBirth"] != DBNull.Value)
                        cc.DateofBirth = DateTime.Parse(reader["DateofBirth"].ToString());

                    if (reader["GenderId"] != DBNull.Value)
                        cc.GenderId = int.Parse(reader["GenderId"].ToString());

                    if (reader["MaritalStatusId"] != DBNull.Value)
                        cc.MaritalStatusId = int.Parse(reader["MaritalStatusId"].ToString());

                    if (reader["PhoneNumber"] != DBNull.Value)
                        cc.PhoneNumber = (reader["PhoneNumber"].ToString());

                    if (reader["EmailAddress"] != DBNull.Value)
                        cc.EmailAddress = (reader["EmailAddress"].ToString());

                    if (reader["CustomerAddress"] != DBNull.Value)
                        cc.CustomerAddress = (reader["CustomerAddress"].ToString());

                    if (reader["NextOfKin"] != DBNull.Value)
                        cc.NextOfKin = (reader["NextOfKin"].ToString());

                    if (reader["CurrentBalance"] != DBNull.Value)
                        cc.CurrentBalance = (reader["CurrentBalance"].ToString());

                    if (reader["RelationshipManagerId"] != DBNull.Value)
                        cc.RelationshipManagerId = int.Parse(reader["RelationshipManagerId"].ToString());

                    if (reader["From"] != DBNull.Value)
                        cc.From = DateTime.Parse(reader["From"].ToString());

                    if (reader["To"] != DBNull.Value)
                        cc.To = DateTime.Parse(reader["To"].ToString());
                    if (reader["Total"] != DBNull.Value)
                        cc.Total = (reader["Total"].ToString());
                    if (reader["Sum"] != DBNull.Value)
                        cc.Sum = (reader["Sum"].ToString());
                    investmentCustomerIndividual.Add(cc);
                }
                con.Close();
            }
            foreach (var item in investmentCustomerIndividual)
            {
                item.AccountOfficer = staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.firstName + " " + staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.lastName;
                item.MaritalStatus = maritalLists.commonLookups.FirstOrDefault(x => x.LookupId == item.MaritalStatusId)?.LookupName;
                item.Gender = genderLists.commonLookups.FirstOrDefault(x => x.LookupId == item.GenderId)?.LookupName;
            }
            return investmentCustomerIndividual;
        }
        public List<InvestmentObj> GetInvestment(DateTime? date1, DateTime? date2)
        {
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            var staffLists = _serverRequest.GetAllStaffAsync().Result;
            var investment = new List<InvestmentObj>();
            using (var con = connection)
            {
                var cmd = new SqlCommand("investment_get_all", con);               
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date2",
                    Value = date2,
                });

                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var cc = new InvestmentObj();

                    if (reader["SN"] != DBNull.Value)
                        cc.SN = int.Parse(reader["SN"].ToString());

                    if (reader["InvestmentID"] != DBNull.Value)
                        cc.InvestmentID = (reader["InvestmentID"].ToString());

                    if (reader["ProductName"] != DBNull.Value)
                        cc.ProductName = (reader["ProductName"].ToString());

                    if (reader["LinkedAccountNumber"] != DBNull.Value)
                        cc.LinkedAccountNumber = (reader["LinkedAccountNumber"].ToString());

                    if (reader["CustomerName"] != DBNull.Value)
                        cc.CustomerName = (reader["CustomerName"].ToString());

                    if (reader["Industry"] != DBNull.Value)
                        cc.Industry = (reader["Industry"].ToString());

                    if (reader["InvestmentDate"] != DBNull.Value)
                        cc.InvestmentDate = DateTime.Parse(reader["InvestmentDate"].ToString());

                    if (reader["MaturityDate"] != DBNull.Value)
                        cc.MaturityDate = DateTime.Parse(reader["MaturityDate"].ToString());

                    if (reader["InvestmentAmount"] != DBNull.Value)
                        cc.InvestmentAmount = (reader["InvestmentAmount"].ToString());

                    if (reader["ApprovedRate"] != DBNull.Value)
                        cc.ApprovedRate = decimal.Parse(reader["ApprovedRate"].ToString());

                    if (reader["ApprovedTenor"] != DBNull.Value)
                        cc.ApprovedTenor = decimal.Parse(reader["ApprovedTenor"].ToString());

                    if (reader["NoOfDaysToMaturity"] != DBNull.Value)
                        cc.NoOfDaysToMaturity = int.Parse(reader["NoOfDaysToMaturity"].ToString());

                    if (reader["InvestmentStatus"] != DBNull.Value)
                        cc.InvestmentStatus = (reader["InvestmentStatus"].ToString());

                    if (reader["RelationshipManagerId"] != DBNull.Value)
                        cc.RelationshipManagerId = int.Parse(reader["RelationshipManagerId"].ToString());

                    if (reader["TotalInterest"] != DBNull.Value)
                        cc.TotalInterest = decimal.Parse(reader["TotalInterest"].ToString());

                    if (reader["From"] != DBNull.Value)
                        cc.From = DateTime.Parse(reader["From"].ToString());

                    if (reader["To"] != DBNull.Value)
                        cc.To = DateTime.Parse(reader["To"].ToString());
                    if (reader["Total"] != DBNull.Value)
                        cc.Total = (reader["Total"].ToString());
                    if (reader["Sum"] != DBNull.Value)
                        cc.Sum = (reader["Sum"].ToString());
                    investment.Add(cc);
                }
                con.Close();
            }
            foreach(var item in investment)
            {
                item.AccountOfficer = staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.firstName + " " + staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.lastName;
                item.InvestmentStatus = item.InvestmentStatus == "0" ? "Pending": item.InvestmentStatus == "1" ? "Running" : item.InvestmentStatus == "2" ? "Matured" : item.InvestmentStatus == "3" ? "Liquidated" : null;
            }

            return investment;
        }
        public List<LoanPaymentSchedulePeriodicObj> GetPeriodicScheduleInvestmentCertificate(string RefNumber)
        {
            var InvestorFundId = (from a in _context.inf_investorfund where a.RefNumber == RefNumber select a.InvestorFundId).FirstOrDefault();
            var data = (from a in _context.inf_investdailyschedule
                        join b in _context.inf_investorfund on a.InvestorFundId equals b.InvestorFundId
                        where a.InvestorFundId == InvestorFundId && a.EndPeriod == true
                        select new LoanPaymentSchedulePeriodicObj
                        {
                            LoanId = a.InvestorFundId ?? 0,
                            PaymentNumber = a.PeriodId ?? 0,
                            PaymentDate = a.PeriodDate.Value.Date,
                            StartPrincipalAmount = (double)a.OB,
                            PeriodPaymentAmount = (double)a.Repayment,
                            PeriodInterestAmount = (double)a.InterestAmount,
                            PeriodPrincipalAmount = (double)a.InterestAmount,
                            EndPrincipalAmount = (double)a.CB,
                            InterestRate = (double)b.ApprovedRate,

                            AmortisedStartPrincipalAmount = (double)a.InterestAmount,
                            AmortisedPeriodPaymentAmount = (double)a.InterestAmount,
                            AmortisedPeriodInterestAmount = (double)a.InterestAmount,
                            AmortisedPeriodPrincipalAmount = (double)a.InterestAmount,
                            AmortisedEndPrincipalAmount = (double)a.InterestAmount,
                            EffectiveInterestRate = (double)b.ApprovedRate,

                        }).ToList();
            return data;
        }
        public OfferLetterDetailObj GenerateInvestmentCertificate(string RefNumber)
        {
            var _Companies = _serverRequest.GetAllCompanyAsync().Result;
            var _Currencies = _serverRequest.GetCurrencyAsync().Result;
            var offerLetterDetails = (from a in _context.inf_investorfund
                                      join b in _context.credit_loancustomer on a.InvestorFundCustomerId equals b.CustomerId
                                      where a.RefNumber == RefNumber && a.Deleted == false &&
                                      a.ApprovalStatus == 2
                                      select new OfferLetterDetailObj
                                      {
                                          ProductName = _context.inf_product.FirstOrDefault(x => x.ProductId == a.ProductId).ProductName,
                                          CustomerName = b.FirstName + " " + b.LastName,
                                          Tenor = (int)a.ApprovedTenor,
                                          InterestRate = (double)a.ApprovedRate,
                                          LoanAmount = a.ApprovedAmount ?? 0,
                                          ExchangeRate = 0,
                                          CustomerAddress = b.Address ?? string.Empty,
                                          CustomerEmailAddress = b.Email,
                                          CustomerPhoneNumber = b.PhoneNo,
                                          ApplicationDate = a.CreatedOn ?? DateTime.Now,
                                          MaturityDate = a.EffectiveDate??DateTime.Now.AddDays((int)a.ApprovedTenor),
                                          LoanApplicationId = a.RefNumber,
                                          RepaymentSchedule = "Not applicable",
                                          RepaymentTerms = "Not applicable",
                                          Purpose = a.InvestmentPurpose,
                                          CompanyId = a.CompanyId,
                                          CurrencyId = a.CurrencyId??0,
                                          GrossRate = calculateGrossRate(a.InvestorFundId, a.ProductId??0),
                                          NetRate = calculateNetRate(a.InvestorFundId, a.ProductId??0),
                                          MaturityAmount = calculateNetPayOut(a.InvestorFundId, a.ProductId??0),
                                      }).FirstOrDefault();

            offerLetterDetails.CompanyName = _Companies.companyStructures.FirstOrDefault(x => x.companyStructureId == offerLetterDetails.CompanyId)?.name;
            if (string.IsNullOrEmpty(offerLetterDetails.CompanyName))
            {
                offerLetterDetails.CompanyName = _Companies.companyStructures.FirstOrDefault(x => x.parentCompanyID == 0)?.name;
            }
            offerLetterDetails.CurrencyName = _Currencies.commonLookups.FirstOrDefault(x => x.LookupId == offerLetterDetails.CurrencyId)?.LookupName;
            return offerLetterDetails;
        }

        ///Credit Reports
        public List<CorporateCustomerObj> GetCreditCustomerCorporate(DateTime? date1, DateTime? date2, int ct)
        {
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            var corporateCustmer = new List<CorporateCustomerObj>();
            var staffLists = _serverRequest.GetAllStaffAsync().Result;
            SqlConnection connection = new SqlConnection(connectionString);
            using (var con = connection)
            {
                var cmd = new SqlCommand("credit_customer_get_all", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date2",
                    Value = date2,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@customerType",
                    Value = ct,
                });

                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var cc = new CorporateCustomerObj();

                    if (reader["SN"] != DBNull.Value)
                        cc.SN = int.Parse(reader["SN"].ToString());

                    if (reader["CustomerId"] != DBNull.Value)
                        cc.CustomerId = int.Parse(reader["CustomerId"].ToString());

                    if (reader["CompanyName"] != DBNull.Value)
                        cc.CompanyName = (reader["CompanyName"].ToString());

                    if (reader["DateOfIncorporation"] != DBNull.Value)
                        cc.DateOfIncorporation = DateTime.Parse(reader["DateOfIncorporation"].ToString());

                    if (reader["RegistrationNumber"] != DBNull.Value)
                        cc.RegistrationNumber = (reader["RegistrationNumber"].ToString());

                    if (reader["PhoneNumber"] != DBNull.Value)
                        cc.PhoneNumber = (reader["PhoneNumber"].ToString());

                    if (reader["Email"] != DBNull.Value)
                        cc.Email = (reader["Email"].ToString());

                    if (reader["Address"] != DBNull.Value)
                        cc.Address = (reader["Address"].ToString());

                    if (reader["CompanyDirector"] != DBNull.Value)
                        cc.CompanyDirector = (reader["CompanyDirector"].ToString());

                    if (reader["PoliticallyExposed"] != DBNull.Value)
                        cc.PoliticallyExposed = (reader["PoliticallyExposed"].ToString());
                    if (reader["Industry"] != DBNull.Value)
                        cc.Industry = (reader["Industry"].ToString());
                    if (reader["IncorporationCountry"] != DBNull.Value)
                        cc.IncorporationCountry = (reader["IncorporationCountry"].ToString());
                    if (reader["AnnualTurnover"] != DBNull.Value)
                        cc.AnnualTurnover = decimal.Parse(reader["AnnualTurnover"].ToString());
                    if (reader["CurrentExposure"] != DBNull.Value)
                        cc.CurrentExposure = decimal.Parse(reader["CurrentExposure"].ToString());
                    if (reader["ExposureLimit"] != DBNull.Value)
                        cc.ExposureLimit = decimal.Parse(reader["ExposureLimit"].ToString());
                    if (reader["ShareholderFund"] != DBNull.Value)
                        cc.ShareholderFund = decimal.Parse(reader["ShareholderFund"].ToString());
                    if (reader["RelationshipManagerId"] != DBNull.Value)
                        cc.RelationshipManagerId = int.Parse(reader["RelationshipManagerId"].ToString());
                    if (reader["Total"] != DBNull.Value)
                        cc.Total = (reader["Total"].ToString());
                    if (reader["Sum"] != DBNull.Value)
                        cc.Sum = (reader["Sum"].ToString());
                    if (reader["From"] != DBNull.Value)
                        cc.From = DateTime.Parse(reader["From"].ToString());
                    if (reader["To"] != DBNull.Value)
                        cc.To = DateTime.Parse(reader["To"].ToString());
                    if (reader["CreatedOn"] != DBNull.Value)
                        cc.CreatedOn = DateTime.Parse(reader["CreatedOn"].ToString());
                    corporateCustmer.Add(cc);
                }
                con.Close();
            }
            foreach (var item in corporateCustmer)
            {
                item.RelationshipManager = staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.firstName + " " + staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.lastName;
            }

            return corporateCustmer;
        }
        public List<IndividualCustomerObj> GetCreditCustomerIndividual(DateTime? date1, DateTime? date2, int ct)
        {
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            var staffLists = _serverRequest.GetAllStaffAsync().Result;
            var genderLists = _serverRequest.GetGenderAsync().Result;
            var maritalLists = _serverRequest.GetMaritalStatusAsync().Result;
            var employmentTypeLists = _serverRequest.GetEmploymentTypeAsync().Result;
            var individualCustomer = new List<IndividualCustomerObj>();
            using (var con = connection)
            {
                var cmd = new SqlCommand("credit_customer_get_all", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date2",
                    Value = date2,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@customerType",
                    Value = ct,
                });

                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var cc = new IndividualCustomerObj();

                    if (reader["SN"] != DBNull.Value)
                        cc.SN = int.Parse(reader["SN"].ToString());

                    if (reader["CustomerId"] != DBNull.Value)
                        cc.CustomerId = int.Parse(reader["CustomerId"].ToString());

                    if (reader["CustomerName"] != DBNull.Value)
                        cc.CustomerName = (reader["CustomerName"].ToString());

                    if (reader["DateOfBirth"] != DBNull.Value)
                        cc.DateOfBirth = DateTime.Parse(reader["DateOfBirth"].ToString());

                    if (reader["GenderId"] != DBNull.Value)
                        cc.GenderId = int.Parse(reader["GenderId"].ToString());

                    if (reader["MaritalStatusId"] != DBNull.Value)
                        cc.MaritalStatusId = int.Parse(reader["MaritalStatusId"].ToString());

                    if (reader["PhoneNo"] != DBNull.Value)
                        cc.PhoneNo = (reader["PhoneNo"].ToString());

                    if (reader["Email"] != DBNull.Value)
                        cc.Email = (reader["Email"].ToString());

                    if (reader["Address"] != DBNull.Value)
                        cc.Address = (reader["Address"].ToString());

                    if (reader["NextOfKin"] != DBNull.Value)
                        cc.NextOfKin = (reader["NextOfKin"].ToString());
                    if (reader["EmploymentType"] != DBNull.Value)
                        cc.EmploymentType = int.Parse(reader["EmploymentType"].ToString());
                    if (reader["Employer"] != DBNull.Value)
                        cc.Employer = (reader["Employer"].ToString());
                    if (reader["CurrentExposure"] != DBNull.Value)
                        cc.CurrentExposure = decimal.Parse(reader["CurrentExposure"].ToString());
                    if (reader["ExposureLimit"] != DBNull.Value)
                        cc.ExposureLimit = decimal.Parse(reader["ExposureLimit"].ToString());
                    if (reader["RelationshipManagerId"] != DBNull.Value)
                        cc.RelationshipManagerId = int.Parse(reader["RelationshipManagerId"].ToString());
                    if (reader["Total"] != DBNull.Value)
                        cc.Total = (reader["Total"].ToString());
                    if (reader["Sum"] != DBNull.Value)
                        cc.Sum = (reader["Sum"].ToString());
                    if (reader["From"] != DBNull.Value)
                        cc.From = DateTime.Parse(reader["From"].ToString());
                    if (reader["To"] != DBNull.Value)
                        cc.To = DateTime.Parse(reader["To"].ToString());
                    if (reader["CreatedOn"] != DBNull.Value)
                        cc.CreatedOn = DateTime.Parse(reader["CreatedOn"].ToString());
                    individualCustomer.Add(cc);
                }
                con.Close();
            }
            foreach(var item in individualCustomer)
            {
                item.RelationshipManager = staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.firstName + " " + staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.lastName;
                item.MaritalStatus = maritalLists.commonLookups.FirstOrDefault(x => x.LookupId == item.MaritalStatusId)?.LookupName;
                item.Gender = genderLists.commonLookups.FirstOrDefault(x => x.LookupId == item.GenderId)?.LookupName;
                item.EmploymentStatus = employmentTypeLists.commonLookups.FirstOrDefault(x => x.LookupId == item.EmploymentType)?.LookupName;
            }
            return individualCustomer;
        }
        public List<LoansObj> GetLoan(DateTime? date1, DateTime? date2)
        {
            string connectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            var loan = new List<LoansObj>();
            var staffLists = _serverRequest.GetAllStaffAsync().Result;
            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("loan_get_all", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date1",
                    Value = date1,
                });

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@date2",
                    Value = date2,
                });


                cmd.CommandTimeout = 0;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var cc = new LoansObj();

                    if (reader["SN"] != DBNull.Value)
                        cc.SN = int.Parse(reader["SN"].ToString());

                    if (reader["LoanRefNumber"] != DBNull.Value)
                        cc.LoanRefNumber = (reader["LoanRefNumber"].ToString());

                    if (reader["ProductName"] != DBNull.Value)
                        cc.ProductName = (reader["ProductName"].ToString());

                    if (reader["LinkedAccountNumber"] != DBNull.Value)
                        cc.LinkedAccountNumber = (reader["LinkedAccountNumber"].ToString());

                    if (reader["CustomerName"] != DBNull.Value)
                        cc.CustomerName = (reader["CustomerName"].ToString());

                    if (reader["Industry"] != DBNull.Value)
                        cc.Industry = (reader["Industry"].ToString());

                    if (reader["DisbursementDate"] != DBNull.Value)
                        cc.DisbursementDate = DateTime.Parse(reader["DisbursementDate"].ToString());

                    if (reader["MaturityDate"] != DBNull.Value)
                        cc.MaturityDate = DateTime.Parse(reader["MaturityDate"].ToString());

                    if (reader["ApplicationAmount"] != DBNull.Value)
                        cc.ApplicationAmount = decimal.Parse(reader["ApplicationAmount"].ToString());

                    if (reader["DisbursedAmount"] != DBNull.Value)
                        cc.DisbursedAmount = decimal.Parse(reader["DisbursedAmount"].ToString());

                    if (reader["ApplicationAmount"] != DBNull.Value)
                        cc.ApplicationAmount = decimal.Parse(reader["ApplicationAmount"].ToString());

                    if (reader["Tenor"] != DBNull.Value)
                        cc.Tenor = decimal.Parse(reader["Tenor"].ToString());

                    if (reader["InterestRate"] != DBNull.Value)
                        cc.InterestRate = decimal.Parse(reader["InterestRate"].ToString());

                    if (reader["TotalInterest"] != DBNull.Value)
                        cc.TotalInterest = decimal.Parse(reader["TotalInterest"].ToString());

                    if (reader["NoOfDaysInOverdue"] != DBNull.Value)
                        cc.NoOfDaysInOverdue = int.Parse(reader["NoOfDaysInOverdue"].ToString());

                    if (reader["ProvisioningRequirement"] != DBNull.Value)
                        cc.ProvisioningRequirement = int.Parse(reader["ProvisioningRequirement"].ToString());

                    if (reader["Description"] != DBNull.Value)
                        cc.Description = (reader["Description"].ToString());


                    if (reader["RepaymentDate"] != DBNull.Value)
                        cc.RepaymentDate = DateTime.Parse(reader["RepaymentDate"].ToString());

                    if (reader["RepaymentAmount"] != DBNull.Value)
                        cc.RepaymentAmount = decimal.Parse(reader["RepaymentAmount"].ToString());

                    if (reader["PAR"] != DBNull.Value)
                        cc.PAR = decimal.Parse(reader["PAR"].ToString());

                    if (reader["OutstandingPrincipal"] != DBNull.Value)
                        cc.OutstandingPrincipal = decimal.Parse(reader["OutstandingPrincipal"].ToString());

                    if (reader["OutstandingInterest"] != DBNull.Value)
                        cc.OutstandingInterest = decimal.Parse(reader["OutstandingInterest"].ToString());

                    if (reader["TotalOutstanding"] != DBNull.Value)
                        cc.TotalOutstanding = decimal.Parse(reader["TotalOutstanding"].ToString());

                    if (reader["PastDuePrincipal"] != DBNull.Value)
                        cc.PastDuePrincipal = decimal.Parse(reader["PastDuePrincipal"].ToString());

                    if (reader["PastDueInterest"] != DBNull.Value)
                        cc.PastDueInterest = decimal.Parse(reader["PastDueInterest"].ToString());

                    if (reader["RelationshipManagerId"] != DBNull.Value)
                        cc.RelationshipManagerId = int.Parse(reader["RelationshipManagerId"].ToString());

                    if (reader["Total"] != DBNull.Value)
                        cc.Total = (reader["Total"].ToString());

                    if (reader["Sum"] != DBNull.Value)
                        cc.Sum = decimal.Parse(reader["Sum"].ToString());
                    if (reader["From"] != DBNull.Value)
                        cc.From = DateTime.Parse(reader["From"].ToString());
                    if (reader["To"] != DBNull.Value)
                        cc.To = DateTime.Parse(reader["To"].ToString());

                    loan.Add(cc);
                }
                con.Close();
            }
            foreach(var item in loan)
            {
                item.AccountOfficer = staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.firstName + " " + staffLists.staff.FirstOrDefault(x => x.staffId == item.RelationshipManagerId)?.lastName;
            }
            return loan;
        }

        public async Task<byte[]> GenerateExportLoan(ReportSummaryObj model)
        {
            try
            {
                DataTable dt = new DataTable();
                Byte[] fileBytes = null;
                if (model.ReportTitle == 1)//Customer Report
                {
                    if (model.CustomerType == 1)// Individual
                    {
                        var employTypeList = await _serverRequest.GetEmploymentTypeAsync();
                        var getMaritalStatusList = await _serverRequest.GetMaritalStatusAsync();
                        var getCountryList = await _serverRequest.GetCountriesAsync();
                        var getGenderList = await _serverRequest.GetGenderAsync();
                        foreach (var item in model.SelectedColumn)
                        {
                            if (item == (int)IndividualCustomerReportEnum.FullName)
                            {
                                dt.Columns.Add("FullName");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.Gender)
                            {
                                dt.Columns.Add("Gender");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.Dob)
                            {
                                dt.Columns.Add("Date of Birth");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.Email)
                            {
                                dt.Columns.Add("Email");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.EmploymentStatus)
                            {
                                dt.Columns.Add("EmploymentStatus");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.City)
                            {
                                dt.Columns.Add("City");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.Country)
                            {
                                dt.Columns.Add("Country");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.Occupation)
                            {
                                dt.Columns.Add("Occupation");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.PoliticallyExposed)
                            {
                                dt.Columns.Add("PoliticallyExposed");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.Phone)
                            {
                                dt.Columns.Add("Phone");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.MaritalStatus)
                            {
                                dt.Columns.Add("MaritalStatus");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.BVN)
                            {
                                dt.Columns.Add("BVN");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.AccountNumber)
                            {
                                dt.Columns.Add("AccountNumber");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.BankName)
                            {
                                dt.Columns.Add("BankName");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.IDIssuer)
                            {
                                dt.Columns.Add("IDIssuer");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.IDNumber)
                            {
                                dt.Columns.Add("IDNumber");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.NextOfKinName)
                            {
                                dt.Columns.Add("NextOfKinName");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.NextOfKinRelationship)
                            {
                                dt.Columns.Add("NextOfKinRelationship");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.NextOfKinEmail)
                            {
                                dt.Columns.Add("NextOfKinEmail");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.NextOfKinPhoneNo)
                            {
                                dt.Columns.Add("NextOfKinPhoneNo");
                            }
                            else if (item == (int)IndividualCustomerReportEnum.NextOfKinAddress)
                            {
                                dt.Columns.Add("NextOfKinAddress");
                            }
                        }

                        var customerList = (from a in _context.credit_loancustomer
                                            where a.Deleted == false && a.CustomerTypeId == 1 
                                            && a.CreatedOn.Value.Date >= model.Date1.Date && a.CreatedOn.Value.Date <= model.Date2.Date//Individual Customers
                                            select new IndividualCustomerObj
                                            {
                                                CustomerName = string.Join(" ", a.FirstName, a.LastName),
                                                GenderId = a.GenderId ?? 0,
                                                DateOfBirth = a.DOB,
                                                Email = a.Email,
                                                EmploymentType = a.EmploymentType ?? 0,
                                                City = a.City,
                                                CountryId = a.CountryId ?? 0,
                                                Occupation = a.Occupation,
                                                PoliticallyExposed = a.PoliticallyExposed ?? false,
                                                PhoneNo = a.PhoneNo,
                                                MaritalStatusId = a.MaritalStatusId ?? 0,
                                                BVN = GetbankDetail(a.CustomerId).bvn,
                                                AccountNumber = GetbankDetail(a.CustomerId).accountNumber,
                                                Bank = GetbankDetail(a.CustomerId).bank,
                                                Issuer = getIDDetails(a.CustomerId).issuer,
                                                IDNumber = getIDDetails(a.CustomerId).IDNumber,
                                                NextOfKinName = GetNextOfKin(a.CustomerId).name,
                                                NextOfKinRelationship = GetNextOfKin(a.CustomerId).relationship,
                                                NextOfKinEmail = GetNextOfKin(a.CustomerId).email,
                                                NextOfKinPhoneNo = GetNextOfKin(a.CustomerId).phone,
                                                NextOfKinAddress = GetNextOfKin(a.CustomerId).address,
                                            }).ToList();

                        foreach (var kk in customerList)
                        {
                            var row = dt.NewRow();
                            foreach (var item in model.SelectedColumn)
                            {
                                var gender = getGenderList.commonLookups.FirstOrDefault(x => x.LookupId == kk.GenderId);
                                if (gender != null)
                                {
                                    kk.Gender = gender.LookupName;
                                }

                                var employmentType = employTypeList.commonLookups.FirstOrDefault(x => x.LookupId == kk.EmploymentType);
                                if (employmentType != null)
                                {
                                    kk.EmploymentStatus = employmentType.LookupName;
                                }

                                var country = getCountryList.commonLookups.Where(x => x.LookupId == kk.CountryId).FirstOrDefault();
                                if (country != null)
                                {
                                    kk.Country = country.LookupName;
                                }

                                var maritalstatus = getMaritalStatusList.commonLookups.Where(x => x.LookupId == kk.MaritalStatusId).FirstOrDefault();
                                if (maritalstatus != null)
                                {
                                    kk.MaritalStatus = maritalstatus.LookupName;
                                }

                                if (kk.DateOfBirth == null)
                                    kk.DateOfBirth = DateTime.Now.Date;
                                
                                if (item == (int)IndividualCustomerReportEnum.FullName)
                                {
                                    row["FullName"] = kk.CustomerName;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.Gender)
                                {
                                    row["Gender"] = kk.Gender;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.Dob)
                                {
                                    row["Date of Birth"] = kk.DateOfBirth;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.Email)
                                {
                                    row["Email"] = kk.Email;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.EmploymentStatus)
                                {
                                    row["EmploymentStatus"] = kk.EmploymentStatus;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.City)
                                {
                                    row["City"] = kk.City;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.Country)
                                {
                                    row["Country"] = kk.Country;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.Occupation)
                                {
                                    row["Occupation"] = kk.Occupation;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.PoliticallyExposed)
                                {
                                    row["PoliticallyExposed"] = kk.PoliticallyExposed;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.Phone)
                                {
                                    row["Phone"] = kk.PhoneNo;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.MaritalStatus)
                                {
                                    row["MaritalStatus"] = kk.MaritalStatus;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.BVN)
                                {
                                    row["BVN"] = kk.BVN;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.AccountNumber)
                                {
                                    row["AccountNumber"] = kk.AccountNumber;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.BankName)
                                {
                                    row["BankName"] = kk.Bank;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.IDIssuer)
                                {
                                    row["IDIssuer"] = kk.Issuer;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.IDNumber)
                                {
                                    row["IDNumber"] = kk.IDNumber;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.NextOfKinName)
                                {
                                    row["NextOfKinName"] = kk.NextOfKinName;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.NextOfKinRelationship)
                                {
                                    row["NextOfKinRelationship"] = kk.NextOfKinRelationship;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.NextOfKinEmail)
                                {
                                    row["NextOfKinEmail"] = kk.NextOfKinEmail;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.NextOfKinPhoneNo)
                                {
                                    row["NextOfKinPhoneNo"] = kk.NextOfKinPhoneNo;
                                }
                                else if (item == (int)IndividualCustomerReportEnum.NextOfKinAddress)
                                {
                                    row["NextOfKinAddress"] = kk.NextOfKinAddress;
                                }
                            }
                            dt.Rows.Add(row);
                        }

                        if (customerList != null)
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("IndividualCustomer");
                                ws.DefaultColWidth = 20;
                                ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                                fileBytes = pck.GetAsByteArray();
                            }
                        }
                        return fileBytes;
                    }
                    else
                    {
                        var getCountryList = await _serverRequest.GetCountriesAsync();

                        foreach (var item in model.SelectedColumn)
                        {
                            if (item == (int)CorporateCustomerReportEnum.CompanyName)
                            {
                                dt.Columns.Add("CompanyName");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.Email)
                            {
                                dt.Columns.Add("Email");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.RegNumber)
                            {
                                dt.Columns.Add("RegNumber");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DateofIncorporation)
                            {
                                dt.Columns.Add("DateofIncorporation");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.PhoneNo)
                            {
                                dt.Columns.Add("PhoneNo");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.Address)
                            {
                                dt.Columns.Add("Address");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.PostalAddress)
                            {
                                dt.Columns.Add("PostalAddress");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.Industry)
                            {
                                dt.Columns.Add("Industry");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.IncorporationCountry)
                            {
                                dt.Columns.Add("IncorporationCountry");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.City)
                            {
                                dt.Columns.Add("City");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.AnnualTurnaover)
                            {
                                dt.Columns.Add("AnnualTurnaover");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.ShareholderFund)
                            {
                                dt.Columns.Add("ShareholderFund");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.CompanyWebsite)
                            {
                                dt.Columns.Add("CompanyWebsite");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.BVN)
                            {
                                dt.Columns.Add("BVN");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.AccountNumber)
                            {
                                dt.Columns.Add("AccountNumber");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.BankName)
                            {
                                dt.Columns.Add("BankName");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorName)
                            {
                                dt.Columns.Add("DirectorName");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorPosition)
                            {
                                dt.Columns.Add("DirectorPosition");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorEmail)
                            {
                                dt.Columns.Add("DirectorEmail");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorDOB)
                            {
                                dt.Columns.Add("DirectorDOB");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorPhone)
                            {
                                dt.Columns.Add("DirectorPhone");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorPercentageShare)
                            {
                                dt.Columns.Add("DirectorPercentageShare");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorType)
                            {
                                dt.Columns.Add("DirectorType");
                            }
                            else if (item == (int)CorporateCustomerReportEnum.DirectorPolicallyExposed)
                            {
                                dt.Columns.Add("DirectorPolicallyExposed");
                            }
                        }
                        var customerList = (from a in _context.credit_loancustomer
                                            where a.Deleted == false && a.CustomerTypeId == 2
                                            && a.CreatedOn.Value.Date >= model.Date1.Date && a.CreatedOn.Value.Date <= model.Date2.Date//Corporate Customers
                                            select new LoanCustomerObj
                                            {
                                                CompanyName = string.Join(" ", a.FirstName, a.LastName),
                                                Email = a.Email,
                                                RegistrationNo = a.RegistrationNo,
                                                Dob = a.DOB,
                                                PhoneNo = a.PhoneNo,
                                                Address = a.Address,
                                                PostalAddress = a.PostaAddress,
                                                Industry = a.Industry,
                                                CountryId = a.CountryId,
                                                City = a.City,
                                                AnnualTurnover = a.AnnualTurnover,
                                                ShareholderFund = a.ShareholderFund,
                                                CompanyWebsite = a.CompanyWebsite,
                                                BVN = GetbankDetail(a.CustomerId).bvn,
                                                AccountNumber = GetbankDetail(a.CustomerId).accountNumber,
                                                BankName = GetbankDetail(a.CustomerId).bank,
                                                DirectorName = getDirectors(a.CustomerId).directorName,
                                                DirectorPosition = getDirectors(a.CustomerId).directorPosition,
                                                DirectorEmail = getDirectors(a.CustomerId).directorEmail,
                                                DirectorDOB = getDirectors(a.CustomerId).directorDOB,
                                                DirectorPhone = getDirectors(a.CustomerId).directorPhone,
                                                DirectorPercentageShare = getDirectors(a.CustomerId).directorPercentShare,
                                                DirectorTypeId = getDirectors(a.CustomerId).directorTypeId,
                                                DirectorPolicallyExposed = getDirectors(a.CustomerId).politicallyExposed
                                            }).ToList();

                        foreach (var kk in customerList)
                        {
                            var row = dt.NewRow();
                            foreach (var item in model.SelectedColumn)
                            {
                                var country = getCountryList.commonLookups.Where(x => x.LookupId == kk.CountryId).FirstOrDefault();
                                if (country != null)
                                {
                                    kk.IncorporationCountry = country.LookupName;
                                }

                                if (kk.Dob == null)
                                    kk.Dob = DateTime.Now.Date;
                                if (kk.DirectorTypeId == 1)
                                {
                                    kk.DirectorType = "Director";
                                } else if (kk.DirectorTypeId == 2)
                                {
                                    kk.DirectorType = "Shareholder";
                                }
                                else
                                {
                                    kk.DirectorType = "Director/Shareholder";
                                }

                                if (item == (int)CorporateCustomerReportEnum.CompanyName)
                                {
                                    row["CompanyName"] = kk.CompanyName;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.Email)
                                {
                                    row["Email"] = kk.Email;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.RegNumber)
                                {
                                    row["RegNumber"] = kk.RegistrationNo;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DateofIncorporation)
                                {
                                    row["DateofIncorporation"] = kk.Dob;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.PhoneNo)
                                {
                                    row["PhoneNo"] = kk.PhoneNo;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.Address)
                                {
                                    row["Address"] = kk.Address;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.PostalAddress)
                                {
                                    row["PostalAddress"] = kk.PostalAddress;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.Industry)
                                {
                                    row["Industry"] = kk.Industry;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.IncorporationCountry)
                                {
                                    row["IncorporationCountry"] = kk.IncorporationCountry;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.City)
                                {
                                    row["City"] = kk.City;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.AnnualTurnaover)
                                {
                                    row["AnnualTurnaover"] = kk.AnnualTurnover;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.ShareholderFund)
                                {
                                    row["ShareholderFund"] = kk.ShareholderFund;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.CompanyWebsite)
                                {
                                    row["CompanyWebsite"] = kk.CompanyWebsite;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.BVN)
                                {
                                    row["BVN"] = kk.BVN;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.AccountNumber)
                                {
                                    row["AccountNumber"] = kk.AccountNumber;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.BankName)
                                {
                                    row["BankName"] = kk.Bank;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorName)
                                {
                                    row["DirectorName"] = kk.DirectorName;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorPosition)
                                {
                                    row["DirectorPosition"] = kk.DirectorPosition;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorEmail)
                                {
                                    row["DirectorEmail"] = kk.DirectorEmail;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorDOB)
                                {
                                    row["DirectorDOB"] = kk.DirectorDOB;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorPhone)
                                {
                                    row["DirectorPhone"] = kk.DirectorPhone;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorPercentageShare)
                                {
                                    row["DirectorPercentageShare"] = kk.DirectorPercentageShare;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorType)
                                {
                                    row["DirectorType"] = kk.DirectorType;
                                }
                                else if (item == (int)CorporateCustomerReportEnum.DirectorPolicallyExposed)
                                {
                                    row["DirectorPolicallyExposed"] = kk.DirectorPolicallyExposed;
                                }
                            }
                            dt.Rows.Add(row);
                        }

                        if (customerList != null)
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            using (ExcelPackage pck = new ExcelPackage())
                            {
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CorporateCustomer");
                                ws.DefaultColWidth = 20;
                                ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                                fileBytes = pck.GetAsByteArray();
                            }
                        }
                        return fileBytes;
                    }
                }
                else if (model.ReportTitle == 2)//Loan Report
                {
                    foreach (var item in model.SelectedColumn)
                    {
                        if (item == (int)LoanReportEnum.LoanNo)
                        {
                            dt.Columns.Add("LoanNo");
                        }
                        else if (item == (int)LoanReportEnum.ProductName)
                        {
                            dt.Columns.Add("ProductName");
                        }
                        else if (item == (int)LoanReportEnum.AccountNumber)
                        {
                            dt.Columns.Add("AccountNumber");
                        }
                        else if (item == (int)LoanReportEnum.AccountName)
                        {
                            dt.Columns.Add("AccountName");
                        }
                        else if (item == (int)LoanReportEnum.Industry)
                        {
                            dt.Columns.Add("Industry");
                        }
                        else if (item == (int)LoanReportEnum.DisbursementDate)
                        {
                            dt.Columns.Add("DisbursementDate");
                        }
                        else if (item == (int)LoanReportEnum.Maturity)
                        {
                            dt.Columns.Add("Maturity");
                        }
                        else if (item == (int)LoanReportEnum.ApplicationAmount)
                        {
                            dt.Columns.Add("ApplicationAmount");
                        }
                        else if (item == (int)LoanReportEnum.DisbursementAmount)
                        {
                            dt.Columns.Add("DisbursementAmount");
                        }
                        else if (item == (int)LoanReportEnum.Tenor)
                        {
                            dt.Columns.Add("Tenor");
                        }
                        else if (item == (int)LoanReportEnum.Rate)
                        {
                            dt.Columns.Add("Rate");
                        }
                        else if (item == (int)LoanReportEnum.TotalInterest)
                        {
                            dt.Columns.Add("TotalInterest");
                        }
                        else if (item == (int)LoanReportEnum.DaysInOverdue)
                        {
                            dt.Columns.Add("DaysInOverdue");
                        }
                        else if (item == (int)LoanReportEnum.LoanApplicationRef)
                        {
                            dt.Columns.Add("LoanApplicationRef");
                        }
                        else if (item == (int)LoanReportEnum.DaysInOverdue)
                        {
                            dt.Columns.Add("DaysInOverdue");
                        }
                        else if (item == (int)LoanReportEnum.Fee)
                        {
                            dt.Columns.Add("Fee");
                        }
                        else if (item == (int)LoanReportEnum.AmortizedBalance)
                        {
                            dt.Columns.Add("AmortizedBalance");
                        }
                        else if (item == (int)LoanReportEnum.LoanBalance)
                        {
                            dt.Columns.Add("LoanBalance");
                        }
                        else if (item == (int)LoanReportEnum.LateRepaymentCharge)
                        {
                            dt.Columns.Add("LateRepaymentCharge");
                        }
                        else if (item == (int)LoanReportEnum.InterestRecievable)
                        {
                            dt.Columns.Add("InterestRecievable");
                        }
                        else if (item == (int)LoanReportEnum.PD)
                        {
                            dt.Columns.Add("PD");
                        }
                        else if (item == (int)LoanReportEnum.CreditScore)
                        {
                            dt.Columns.Add("CreditScore");
                        }
                        else if (item == (int)LoanReportEnum.OutstandingTenor)
                        {
                            dt.Columns.Add("OutstandingTenor");
                        }
                        else if (item == (int)LoanReportEnum.EIR)
                        {
                            dt.Columns.Add("EIR");
                        }
                        else if (item == (int)LoanReportEnum.Collateral_YES_OR_NO)
                        {
                            dt.Columns.Add("Collateral");
                        }
                        else if (item == (int)LoanReportEnum.CollateralPercentage)
                        {
                            dt.Columns.Add("CollateralPercentage");
                        }
                        else if (item == (int)LoanReportEnum.Restructured)
                        {
                            dt.Columns.Add("Restructured");
                        }
                    }

                    var loanList = (from a in _context.credit_loan 
                                    join b in _context.credit_loancustomer on a.CustomerId equals b.CustomerId
                                    join c in _context.credit_loanapplication on a.LoanApplicationId equals c.LoanApplicationId
                                    join d in _context.credit_product on a.ProductId equals d.ProductId
                                    where a.Deleted == false && a.IsDisbursed == true
                                    && b.CreatedOn.Value.Date >= model.Date1.Date && b.CreatedOn.Value.Date <= model.Date2.Date//Loans
                                    select new LoanObj
                                    {
                                        LoanRefNumber = a.LoanRefNumber,                                       
                                        ProductName = d.ProductName,
                                        CustomerName = string.Join(" ", b.FirstName, b.LastName),
                                        CasaAccountNumber = b.CASAAccountNumber,
                                        Industry = b.Industry,
                                        DisburseDate = a.DisbursedDate,
                                        MaturityDate = a.MaturityDate,
                                        RequestedAmount = c.ProposedAmount,
                                        ApprovedAmount = c.ApprovedAmount,
                                        Teno = c.ApprovedTenor,
                                        InterestRate = a.InterestRate,
                                        TotalAmount = _context.credit_loanscheduleperiodic.Where(x=>x.LoanId == a.LoanId && x.Deleted == true).Sum(x=>x.PeriodInterestAmount),
                                        DaysInOverdue = ((TimeSpan)(DateTime.Today - (_context.credit_loan_past_due.FirstOrDefault(x => x.LoanId == a.LoanId).Date))).Days,
                                        ApplicationReferenceNumber = c.ApplicationRefNumber,
                                        IntegralFeeAmount = a.IntegralFeeAmount??0,
                                        PrincipalAmount = a.PrincipalAmount,///amostize
                                        LoanPrincipal = a.OutstandingPrincipal??0,//loanbalance
                                        LateRepaymentCharge = a.LateRepaymentCharge,
                                        InterestAmountIR = a.OutstandingInterest??0,
                                        PD = c.PD??0,
                                        CreditScore = _context.credit_creditrating.FirstOrDefault(x => x.MinRange < c.PD && x.MaxRange > c.PD).Rate,
                                        OutstandingTenor = ((TimeSpan)(DateTime.Today - a.EffectiveDate.Date)).Days,
                                        EIR = _context.credit_loanscheduleperiodic.FirstOrDefault(x=>x.LoanId == a.LoanId).EffectiveInterestRate,
                                        Collateral = d.CollateralRequired == true ? "Yes" : "No",
                                        CollateralPercentage = d.CollateralPercentage??0,
                                        Restructured = _context.credit_loanscheduleperiodic.Count(x => x.LoanId == a.LoanId && x.Deleted == true) > 0 ? true : false,
                                    }).ToList();

                    foreach (var kk in loanList)
                    {
                        var row = dt.NewRow();
                        foreach (var item in model.SelectedColumn)
                        {
                            if (item == (int)LoanReportEnum.LoanNo)
                            {
                                row["LoanNo"] = kk.LoanRefNumber;
                            }
                            else if (item == (int)LoanReportEnum.ProductName)
                            {
                                row["ProductName"] = kk.ProductName;
                            }
                            else if (item == (int)LoanReportEnum.AccountNumber)
                            {
                                row["AccountNumber"] = kk.CasaAccountNumber;
                            }
                            else if (item == (int)LoanReportEnum.AccountName)
                            {
                                row["AccountName"] = kk.CustomerName;
                            }
                            else if (item == (int)LoanReportEnum.Industry)
                            {
                                row["Industry"] = kk.Industry;
                            }
                            else if (item == (int)LoanReportEnum.DisbursementDate)
                            {
                                row["DisbursementDate"] = kk.DisburseDate;
                            }
                            else if (item == (int)LoanReportEnum.Maturity)
                            {
                                row["Maturity"] = kk.MaturityDate;
                            }
                            else if (item == (int)LoanReportEnum.ApplicationAmount)
                            {
                                row["ApplicationAmount"] = kk.RequestedAmount;
                            }
                            else if (item == (int)LoanReportEnum.DisbursementAmount)
                            {
                                row["DisbursementAmount"] = kk.ApprovedAmount;
                            }
                            else if (item == (int)LoanReportEnum.Tenor)
                            {
                                row["Tenor"] = kk.Teno;
                            }
                            else if (item == (int)LoanReportEnum.Rate)
                            {
                                row["Rate"] = kk.InterestRate;
                            }
                            else if (item == (int)LoanReportEnum.TotalInterest)
                            {
                                row["TotalInterest"] = kk.TotalAmount;
                            }
                            else if (item == (int)LoanReportEnum.DaysInOverdue)
                            {
                                row["DaysInOverdue"] = kk.DaysInOverdue;
                            }
                            else if (item == (int)LoanReportEnum.LoanApplicationRef)
                            {
                                row["LoanApplicationRef"] = kk.ApplicationReferenceNumber;
                            }
                            else if (item == (int)LoanReportEnum.Fee)
                            {
                                row["Fee"] = kk.IntegralFeeAmount;
                            }
                            else if (item == (int)LoanReportEnum.AmortizedBalance)
                            {
                                row["AmortizedBalance"] = kk.PrincipalAmount;
                            }
                            else if (item == (int)LoanReportEnum.LoanBalance)
                            {
                                row["LoanBalance"] = kk.LoanPrincipal;
                            }
                            else if (item == (int)LoanReportEnum.LateRepaymentCharge)
                            {
                                row["LateRepaymentCharge"] = kk.LateRepaymentCharge;
                            }
                            else if (item == (int)LoanReportEnum.InterestRecievable)
                            {
                                row["InterestRecievable"] = kk.InterestAmountIR;
                            }
                            else if (item == (int)LoanReportEnum.PD)
                            {
                                row["PD"] = kk.PD;
                            }
                            else if (item == (int)LoanReportEnum.CreditScore)
                            {
                                row["CreditScore"] = kk.CreditScore;
                            }
                            else if (item == (int)LoanReportEnum.OutstandingTenor)
                            {
                                row["OutstandingTenor"] = kk.OutstandingTenor;
                            }
                            else if (item == (int)LoanReportEnum.EIR)
                            {
                                row["EIR"] = kk.EIR;
                            }
                             else if (item == (int)LoanReportEnum.Collateral_YES_OR_NO)
                            {
                                row["Collateral"] = kk.Collateral;
                            }
                            else if (item == (int)LoanReportEnum.CollateralPercentage)
                            {
                                row["CollateralPercentage"] = kk.CollateralPercentage;
                            }
                            else if (item == (int)LoanReportEnum.Restructured)
                            {
                                row["Restructured"] = kk.Restructured == true ? "Yes" : "No";
                            }
                        }
                        dt.Rows.Add(row);
                    }                   

                    if (loanList != null)
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage pck = new ExcelPackage())
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Loan");
                            ws.DefaultColWidth = 20;
                            ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                            fileBytes = pck.GetAsByteArray();
                        }
                    }
                    return fileBytes;
                }
                else if (model.ReportTitle == 3)//Repayment Report
                {
                    dt.Columns.Add("PaymentNumber");
                    dt.Columns.Add("PaymentDate");
                    dt.Columns.Add("StartPrincipalAmount");
                    dt.Columns.Add("PeriodPaymentAmount");
                    dt.Columns.Add("PeriodInterestAmount");
                    dt.Columns.Add("PeriodPrincipalAmount");
                    dt.Columns.Add("EndPrincipalAmount");
                    dt.Columns.Add("InterestRate");
                    dt.Columns.Add("AmortisedStartPrincipalAmount");
                    dt.Columns.Add("AmortisedPeriodPaymentAmount");
                    dt.Columns.Add("AmortisedPeriodInterestAmount");
                    dt.Columns.Add("AmortisedPeriodPrincipalAmount");
                    dt.Columns.Add("AmortisedEndPrincipalAmount");
                    dt.Columns.Add("EffectiveInterestRate");

                    var loanList = (from a in _context.credit_loan
                                    join b in _context.credit_loanscheduleperiodic on a.LoanId equals b.LoanId
                                    where a.Deleted == false && a.IsDisbursed == true && a.LoanRefNumber == model.LoanId //Loans
                                    select new LoanPaymentSchedulePeriodicObj
                                    {
                                        PaymentNumber = b.PaymentNumber,
                                        PaymentDate = b.PaymentDate.Date,
                                        StartPrincipalAmount = Convert.ToDouble(b.StartPrincipalAmount),
                                        PeriodPaymentAmount = Convert.ToDouble(b.PeriodPaymentAmount),
                                        PeriodInterestAmount = Convert.ToDouble(b.PeriodInterestAmount),
                                        PeriodPrincipalAmount = Convert.ToDouble(b.PeriodPrincipalAmount),
                                        EndPrincipalAmount = Convert.ToDouble(b.EndPrincipalAmount),
                                        InterestRate = a.InterestRate,
                                        AmortisedStartPrincipalAmount = Convert.ToDouble(b.AmortisedStartPrincipalAmount),
                                        AmortisedPeriodPaymentAmount = Convert.ToDouble(b.AmortisedPeriodPaymentAmount),
                                        AmortisedPeriodInterestAmount = Convert.ToDouble(b.AmortisedPeriodInterestAmount),
                                        AmortisedPeriodPrincipalAmount = Convert.ToDouble(b.AmortisedPeriodPrincipalAmount),
                                        AmortisedEndPrincipalAmount = Convert.ToDouble(b.AmortisedEndPrincipalAmount),
                                        EffectiveInterestRate = b.EffectiveInterestRate,
                                    }).ToList();


                    foreach (var kk in loanList)
                    {
                        var row = dt.NewRow();
                        row["PaymentNumber"] = kk.PaymentNumber;
                        row["PaymentDate"] = kk.PaymentDate.Date;
                        row["StartPrincipalAmount"] = kk.StartPrincipalAmount;
                        row["PeriodPaymentAmount"] = kk.PeriodPaymentAmount;
                        row["PeriodInterestAmount"] = kk.PeriodInterestAmount;
                        row["PeriodPrincipalAmount"] = kk.PeriodPrincipalAmount;
                        row["EndPrincipalAmount"] = kk.EndPrincipalAmount;
                        row["InterestRate"] = kk.InterestRate;
                        row["AmortisedStartPrincipalAmount"] = kk.AmortisedStartPrincipalAmount;
                        row["AmortisedPeriodPaymentAmount"] = kk.AmortisedPeriodPaymentAmount;
                        row["AmortisedPeriodInterestAmount"] = kk.AmortisedPeriodInterestAmount;
                        row["AmortisedPeriodPrincipalAmount"] = kk.AmortisedPeriodPrincipalAmount;
                        row["AmortisedEndPrincipalAmount"] = kk.AmortisedEndPrincipalAmount;
                        row["EffectiveInterestRate"] = kk.EffectiveInterestRate;

                        dt.Rows.Add(row);
                    }

                if (loanList != null)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("loanSchedule");
                        ws.DefaultColWidth = 20;
                        ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                        fileBytes = pck.GetAsByteArray();
                    }
                }
                return fileBytes;
            }
                else if (model.ReportTitle == 4)//Restructured Report
                {
                    return fileBytes;
                }
                return fileBytes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class directors
        {
            public string directorName { get; set; }
            public string directorPosition { get; set; }
            public string directorEmail { get; set; }
            public DateTime directorDOB { get; set; }
            public string directorPhone { get; set; }
            public decimal directorPercentShare { get; set; }
            public int directorTypeId { get; set; }
            public bool politicallyExposed { get; set; }
        }
        public class nextOfKin
        {
            public string name { get; set; }
            public string email { get; set; }
            public string relationship { get; set; }
            public string phone { get; set; }
            public string address { get; set; }
        }

        public class bankDetail
        {
            public string accountNumber { get; set; }
            public string bank { get; set; }
            public string bvn { get; set; }
        }


        public class IDDetails
        {
            public string issuer { get; set; }
            public string IDNumber { get; set; }
        }

        static IDDetails getIDDetails(int id)
        {
            DataContext context = new DataContext();
            var data = context.credit_customeridentitydetails.FirstOrDefault(s => s.Deleted == false && s.CustomerId == id);
            context.Dispose();
            if (data == null)
                return new IDDetails();
            return new IDDetails { IDNumber = data.Number, issuer = data.Issuer};
        }

        static bankDetail GetbankDetail(int id)
        {
            DataContext context = new DataContext();
            var data = context.credit_customerbankdetails.FirstOrDefault(s => s.Deleted == false && s.CustomerId == id);
            context.Dispose();
            if (data == null)
                return new bankDetail();
            return new bankDetail { accountNumber = data.Account, bank = data.Bank, bvn = data.BVN};
        }

        static nextOfKin GetNextOfKin(int id)
        {
            DataContext context = new DataContext();
            var data = context.credit_customernextofkin.FirstOrDefault(s => s.Deleted == false && s.CustomerId == id);
            context.Dispose();
            if (data == null)
                return new nextOfKin();
            return new nextOfKin { email = data.Email, name = data.Name, relationship = data.Relationship, address = data.Address, phone = data.PhoneNo };
        }

        static directors getDirectors(int id)
        {
            DataContext context = new DataContext();
            var data = context.credit_loancustomerdirector.FirstOrDefault(s => s.Deleted == false && s.CustomerId == id);
            context.Dispose();
            if (data == null)
                return new directors();
            return new directors { directorName = data.Name, directorEmail = data.Email, directorPhone = data.PhoneNo, politicallyExposed = data.PoliticallyPosition, directorTypeId = data.DirectorTypeId, directorPosition = data.Position };
        }

        static decimal calculateNetRate(int investorfundId,int productId)
        {
            DataContext context = new DataContext();
            var product = context.inf_product.Find(productId);
            var investorFund = context.inf_investorfund.Find(investorfundId);
            var payout = context.inf_investdailyschedule.Where(x => x.InvestorFundId == investorfundId).OrderByDescending(l => l.Period).FirstOrDefault().Repayment;
            var grossInterest = payout - investorFund.ApprovedAmount;
            var witholdingTax = (product.TaxRate / 100) * grossInterest;
            var netInterest = grossInterest - witholdingTax;
            var expectedPayout = investorFund.ApprovedAmount + netInterest;
            context.Dispose();
            return netInterest??0;
        }

        static decimal calculateNetPayOut(int investorfundId, int productId)
        {
            DataContext context = new DataContext();
            var product = context.inf_product.Find(productId);
            var investorFund = context.inf_investorfund.Find(investorfundId);
            var payout = context.inf_investdailyschedule.Where(x => x.InvestorFundId == investorfundId).OrderByDescending(l => l.Period).FirstOrDefault().Repayment;
            var grossInterest = payout - investorFund.ApprovedAmount;
            var witholdingTax = (product.TaxRate / 100) * grossInterest;
            var netInterest = grossInterest - witholdingTax;
            var expectedPayout = investorFund.ApprovedAmount + netInterest;
            context.Dispose();
            return expectedPayout??0;
        }

        static decimal calculateGrossRate(int investorfundId, int productId)
        {
            DataContext context = new DataContext();
            var product = context.inf_product.Find(productId);
            var investorFund = context.inf_investorfund.Find(investorfundId);
            var payout = context.inf_investdailyschedule.Where(x => x.InvestorFundId == investorfundId).OrderByDescending(l => l.Period).FirstOrDefault().Repayment;
            var grossInterest = payout - investorFund.ApprovedAmount;
            context.Dispose();
            return grossInterest??0;
        }
    }
}
