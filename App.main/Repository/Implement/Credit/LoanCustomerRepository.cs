﻿using Banking.Contracts.Response.Credit;
using Banking.Data;
using Banking.DomainObjects.Credit;
using Banking.Repository.Interface.Credit;
using GODP.Entities.Models;
using GOSLibraries.Enums;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Banking.DomainObjects.Auth;
using GOSLibraries.GOS_API_Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Banking.Requests;
using Banking.Contracts.Response.FlutterWave;

namespace Banking.Repository.Implement.Credit
{
    public class LoanCustomerRepository : ILoanCustomerRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServerRequest _identityServer;
        private readonly IFlutterWaveRequest _flutter;

        public LoanCustomerRepository(DataContext context, UserManager<ApplicationUser> userManager, IIdentityServerRequest identityServer, IFlutterWaveRequest flutter)
        {
            _context = context;
            _userManager = userManager;
            _identityServer = identityServer;
            _flutter = flutter;
        }

        public bool AddUpdateDocumentType(DocumentTypeObj entity)
        {
            try
            {

                if (entity == null) return false;


                if (entity.DocumentTypeId > 0)
                {
                    var documentTypeExist = _context.credit_documenttype.Find(entity.DocumentTypeId);
                    if (documentTypeExist != null)
                    {
                        documentTypeExist.Name = entity.DocumentTypeName;
                        documentTypeExist.Active = true;
                        documentTypeExist.Deleted = false;
                        documentTypeExist.UpdatedBy = entity.CreatedBy;
                        documentTypeExist.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    var documentType = new credit_documenttype
                    {
                        Name = entity.DocumentTypeName,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    _context.credit_documenttype.Add(documentType);
                }

                var response = _context.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteDocumentType(int documentTypeId)
        {
            try
            {
                var documentType = _context.credit_documenttype.Find(documentTypeId);
                if (documentType != null)
                {
                    documentType.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomer = _context.credit_loancustomer.Find(loanCustomerId);
                if (loanCustomer != null)
                {
                    loanCustomer.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanCustomerBankDetails(int loanCustomerBankDetailsId)
        {
            try
            {
                var loanCustomerBankDetails = _context.credit_customerbankdetails.Find(loanCustomerBankDetailsId);
                if (loanCustomerBankDetails != null)
                {
                    loanCustomerBankDetails.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanCustomerDirector(int loanCustomerDirectorId)
        {
            try
            {
                var loanCustomerDirector = _context.credit_loancustomerdirector.Find(loanCustomerDirectorId);
                if (loanCustomerDirector != null)
                {
                    loanCustomerDirector.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanCustomerDirectorShareHolder(int loanCustomerDirectorShareHolderId)
        {
            try
            {
                var loanCustomerDirectorShareHolder = _context.credit_directorshareholder.Find(loanCustomerDirectorShareHolderId);
                if (loanCustomerDirectorShareHolder != null)
                {
                    loanCustomerDirectorShareHolder.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanCustomerDocument(int loanCustomerDocumentId)
        {

            try
            {
                var loanCustomerDocument = _context.credit_loancustomerdocument.Find(loanCustomerDocumentId);
                if (loanCustomerDocument != null)
                {
                    loanCustomerDocument.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanCustomerIdentityDetails(int loanCustomerIdentityDetailsId)
        {
            try
            {
                var loanCustomerIdentityDetails = _context.credit_customeridentitydetails.Find(loanCustomerIdentityDetailsId);
                if (loanCustomerIdentityDetails != null)
                {
                    loanCustomerIdentityDetails.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteLoanCustomerNextOfKin(int loanCustomerNextOfKinId)
        {
            try
            {
                var loanCustomerNextOfKin = _context.credit_customernextofkin.Find(loanCustomerNextOfKinId);
                if (loanCustomerNextOfKin != null)
                {
                    loanCustomerNextOfKin.Deleted = true;
                }
                return _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool EmailExists(string email)
        {
            return _context.credit_loancustomer.Any(x => x.Email.ToLower().Trim() == email.ToLower().Trim());
        }

        public bool ForgotPassword(string email)
        {
            try
            {
                Guid guid = Guid.NewGuid();
                var customer = _context.credit_loancustomer.Where(x => x.Email.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault();

                bool output = false;
                //var accountId = customer.CustomerId;
                //var baseFrontendURL = mail_config.BaseFrontEndURL;
                //StringBuilder sbEmailBody = new StringBuilder();
                //sbEmailBody.Append("<td style =\"font-family: sans-serif; font-size: 14px; vertical-align: top;>\"");
                //sbEmailBody.Append("<p style =\"font -family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\">");
                //sbEmailBody.Append($"Hi {customer.FirstName},");
                //sbEmailBody.Append($"<br/><br/><br/></p><p style =\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\"> You recently requested to reset your password for {mail_config.MailCaption} account.  Click the button below to reset it.</p>");
                //sbEmailBody.Append($"<br/></p><p style =\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\"> Please be informed that the password link is active for 1 hour.</p>");
                //sbEmailBody.Append("<table border =\"0\" cellpadding =\"0\" cellspacing =\"0\" class=\"btn btn-primary\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100 %; box-sizing: border-box;\"><tbody><tr>");
                //sbEmailBody.Append("<td align =\"left\" style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; padding-bottom: 15px;\">");
                //sbEmailBody.Append("<table border = \"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;\">");
                //sbEmailBody.Append("<tbody><tr>");
                //sbEmailBody.Append($"<td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #3498db; border-radius: 5px; text-align: center;\"><a href=\"{baseFrontendURL + "/#/auth/reset-password/" + guid.ToString() }\" target=\"_blank\" style=\"display: inline-block; color: #ffffff; background-color: #3498db; border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-transform: capitalize; border-color: #3498db;\">Reset Password</a></td></tr>");

                //sbEmailBody.Append("</tbody></table></td></tr></tbody>");
                //sbEmailBody.Append("</table>");
                //sbEmailBody.Append("<p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\"> If you did not request a password reset, please ignore this email or reply to let us know.</p>");
                //sbEmailBody.Append("<br/><br/><br/><p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\"> Regards!</p>");
                //sbEmailBody.Append("</td>");

                //var mail = new EmailHelpers();
                //output = mail.SendMail(mail_config, customer.Email, null, "Forget Password", sbEmailBody.ToString(), "~/EmailTemplate/confirmation.html");
                //if (output)
                //{
                //    var resetPassword = new cor_resetpassword
                //    {
                //        UserAccountId = customer.CustomerId,
                //        Guid = guid.ToString(),
                //        Email = customer.Email,
                //        StartTime = DateTime.Now,
                //        EndTime = DateTime.Now.AddHours(1),
                //        ResetPassword = false,
                //    };
                //    _context.cor_resetpassword.Add(resetPassword);
                //    _context.SaveChanges();
                //}
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] GenerateExportDocumentType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Document Name");

            var statementType = (from a in _context.credit_documenttype
                                 where a.Deleted == false
                                 select new DocumentTypeObj
                                 {
                                     DocumentTypeId = a.DocumentTypeId,

                                 }).ToList();

            foreach (var kk in statementType)
            {
                var row = dt.NewRow();
                kk.DocumentTypeName = _context.credit_documenttype.Where(x => x.DocumentTypeId == kk.DocumentTypeId).FirstOrDefault().Name;
                row["Document Name"] = kk.DocumentTypeName;

                dt.Rows.Add(row);
            }
            Byte[] fileBytes = null;

            if (statementType != null)
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Document Type");
                    ws.DefaultColWidth = 20;
                    ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    fileBytes = pck.GetAsByteArray();
                }
            }
            return fileBytes;
        }

        public async Task<byte[]> GenerateExportLoanCustomer()
        {
            try
            {
                var titleList = await _identityServer.GetTitleAsync();
                var employTypeList = await _identityServer.GetEmploymentTypeAsync();
                var getMaritalStatusList = await _identityServer.GetMaritalStatusAsync();
                //var getCityList = await _identityServer.GetCitiesAsync();
                var getCountryList = await _identityServer.GetCountriesAsync();
                var getGenderList = await _identityServer.GetGenderAsync();

                DataTable dt = new DataTable();
                dt.Columns.Add("Email");
                dt.Columns.Add("Customer Type Name");
                dt.Columns.Add("Title");
                dt.Columns.Add("First Name");
                dt.Columns.Add("Middle Name");
                dt.Columns.Add("Last Name");              
                dt.Columns.Add("Gender");
                dt.Columns.Add("Address");
                dt.Columns.Add("Date Of Birth");            
                dt.Columns.Add("Employment Type");
                dt.Columns.Add("City");
                dt.Columns.Add("Occupation");
                dt.Columns.Add("Politically Exposed");
                dt.Columns.Add("Country");
                dt.Columns.Add("Phone Number");
                dt.Columns.Add("Marital Status");
                var statementType = (from a in _context.credit_loancustomer
                                     where a.Deleted == false && a.CustomerTypeId == 1
                                     select new LoanCustomerObj
                                     {
                                         CustomerId = a.CustomerId,
                                         CustomerTypeId = a.CustomerTypeId,
                                         TitleId = a.TitleId,
                                         FirstName = a.FirstName,
                                         LastName = a.LastName,
                                         MiddleName = a.MiddleName,
                                         GenderId = a.GenderId,
                                         Address = a.Address,
                                         Dob = a.DOB,
                                         PostalAddress = a.PostaAddress,
                                         Email = a.Email,
                                         EmploymentType = a.EmploymentType,
                                         IncorporationCountry = a.IncorporationCountry,
                                         Industry = a.Industry,
                                         ShareholderFund = a.ShareholderFund,
                                         CityId = a.CityId,
                                         City = a.City,
                                         Occupation = a.Occupation,
                                         PoliticallyExposed = a.PoliticallyExposed,
                                         CompanyName = a.CompanyName,
                                         CompanyWebsite = a.CompanyWebsite,
                                         RegistrationNo = a.RegistrationNo,
                                         AnnualTurnover = a.AnnualTurnover,
                                         CountryId = a.CountryId,
                                         PhoneNo = a.PhoneNo,
                                         MaritalStatusId = a.MaritalStatusId,
                                         ApprovalStatusId = a.ApprovalStatusId,
                                         RelationshipOfficerId = a.RelationshipManagerId
                                     }).ToList();

                foreach (var kk in statementType)
                {
                    var row = dt.NewRow();
                    var titlename = "";
                    var title = titleList.commonLookups.Where(x => x.LookupId == kk.TitleId).FirstOrDefault();
                    if (title == null)
                    {
                        titlename = "";
                    }
                    else
                    {
                        titlename = title.LookupName;
                    }

                    var gendername = "";
                    var gender = getGenderList.commonLookups.Where(x => x.LookupId == kk.GenderId).FirstOrDefault();
                    if (gender == null)
                    {
                        gendername = "";
                    }
                    else
                    {
                        gendername = gender.LookupName;
                    }

                    var politicallyexposedname = false;
                    var politicallyexposed = kk.PoliticallyExposed;
                    if (politicallyexposed == null)
                    {
                        politicallyexposedname = false;
                    }
                    else
                    {
                        politicallyexposedname = politicallyexposed ?? false;
                    }

                    var employmenttypename = "";
                    var employmentType = employTypeList.commonLookups.Where(x => x.LookupId == kk.EmploymentType).FirstOrDefault();
                    if (employmentType == null)
                    {
                        employmenttypename = "";
                    }
                    else
                    {
                        employmenttypename = employmentType.LookupName;
                    }

                    //var cityname = "";
                    //var city = getCityList.commonLookups.Where(x => x.LookupId == kk.CityId).FirstOrDefault();
                    //if (city == null)
                    //{
                    //    cityname = "";
                    //}
                    //else
                    //{
                    //    cityname = city.LookupName;
                    //}

                    var countryname = "";
                    var country = getCountryList.commonLookups.Where(x => x.LookupId == kk.CountryId).FirstOrDefault();
                    if (country == null)
                    {
                        countryname = "";
                    }
                    else
                    {
                        countryname = country.LookupName;
                    }

                    var maritalstatusname = "";
                    var maritalstatus = getMaritalStatusList.commonLookups.Where(x => x.LookupId == kk.MaritalStatusId).FirstOrDefault();
                    if (maritalstatus == null)
                    {
                        maritalstatusname = "";
                    }
                    else
                    {
                        maritalstatusname = maritalstatus.LookupName;
                    }

                    var customertypename = "";
                    customertypename = kk.CustomerTypeId == 1 ? "Individual" : "Corporate";
                    if (kk.Dob == null)
                        kk.Dob = DateTime.Now.Date;

                    row["Email"] = kk.Email;
                    row["Customer Type Name"] = customertypename;
                    row["Title"] = titlename;
                    row["First Name"] = kk.FirstName;
                    row["Middle Name"] = kk.MiddleName;
                    row["Last Name"] = kk.LastName;                   
                    row["Gender"] = gendername;
                    row["Address"] = kk.Address;
                    row["Date of Birth"] = kk.Dob.Value.Date;                   
                    row["Employment Type"] = employmenttypename;
                    row["City"] = kk.City;
                    row["Occupation"] = kk.Occupation;
                    row["Politically Exposed"] = kk.PoliticallyExposed;
                    row["Country"] = countryname;
                    row["Phone Number"] = kk.PhoneNo;
                    row["Marital Status"] = maritalstatusname;
                                       
                    dt.Rows.Add(row);

                }
                Byte[] fileBytes = null;

                if (statementType != null)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("LoanCustomer");
                        ws.DefaultColWidth = 20;
                        ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                        fileBytes = pck.GetAsByteArray();
                    }
                }
                return fileBytes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<byte[]> GenerateExportCorporateLoanCustomer()
        {
            try
            {
                var getCityList = await _identityServer.GetCitiesAsync();
                var getCountryList = await _identityServer.GetCountriesAsync();

                DataTable dt = new DataTable();
                dt.Columns.Add("Email");
                dt.Columns.Add("Customer Type Name");
                dt.Columns.Add("Registration Number");
                dt.Columns.Add("Company Name");
                dt.Columns.Add("Date Of Incorporation");
                dt.Columns.Add("Phone Number");
                dt.Columns.Add("Address");
                dt.Columns.Add("Postal Address");             
                dt.Columns.Add("Industry");
                dt.Columns.Add("Incorporation Country");
                dt.Columns.Add("City");
                dt.Columns.Add("Annual Turnover");
                dt.Columns.Add("Shareholder Fund");
                dt.Columns.Add("Company Website");
                

                var customerList = (from a in _context.credit_loancustomer
                                     where a.Deleted == false && a.CustomerTypeId == 2 //Corporate Customers
                                     select new LoanCustomerObj
                                     {
                                         CustomerId = a.CustomerId,
                                         CustomerTypeId = a.CustomerTypeId,
                                         TitleId = a.TitleId,
                                         FirstName = a.FirstName,
                                         LastName = a.LastName,
                                         MiddleName = a.MiddleName,
                                         GenderId = a.GenderId,
                                         Address = a.Address,
                                         Dob = a.DOB,
                                         PostalAddress = a.PostaAddress,
                                         Email = a.Email,
                                         EmploymentType = a.EmploymentType,
                                         IncorporationCountry = a.IncorporationCountry,
                                         Industry = a.Industry,
                                         ShareholderFund = a.ShareholderFund,
                                         CityId = a.CityId,
                                         City = a.City,
                                         Occupation = a.Occupation,
                                         PoliticallyExposed = a.PoliticallyExposed,
                                         CompanyName = a.CompanyName,
                                         CompanyWebsite = a.CompanyWebsite,
                                         RegistrationNo = a.RegistrationNo,
                                         AnnualTurnover = a.AnnualTurnover,
                                         CountryId = a.CountryId,
                                         PhoneNo = a.PhoneNo,
                                         MaritalStatusId = a.MaritalStatusId,
                                         ApprovalStatusId = a.ApprovalStatusId,
                                         RelationshipOfficerId = a.RelationshipManagerId
                                     }).ToList();

                foreach (var kk in customerList)
                {
                    var row = dt.NewRow();
                    var country = getCountryList.commonLookups.Where(x => x.LookupId == kk.CountryId).FirstOrDefault();
                    if (country != null)
                    {
                        kk.IncorporationCountry = country.LookupName;
                    }

                    var customertypename = "";
                    customertypename = kk.CustomerTypeId == 1 ? "Individual" : "Corporate";
                    if (kk.Dob == null)
                        kk.Dob = DateTime.Now.Date;

                    row["Email"] = kk.Email;
                    row["Customer Type Name"] = customertypename;
                    row["Registration Number"] = kk.RegistrationNo;
                    row["Company Name"] = kk.CompanyName;
                    row["Date Of Incorporation"] = kk.Dob;                   
                    row["Phone Number"] = kk.PhoneNo;
                    row["Address"] = kk.PostalAddress;
                    row["Postal Address"] = kk.PostalAddress;
                    row["Industry"] = kk.Industry;
                    row["Incorporation Country"] = kk.IncorporationCountry;                   
                    row["City"] = kk.City;
                    row["Annual Turnover"] = kk.AnnualTurnover;
                    row["Shareholder Fund"] = kk.ShareholderFund;
                    row["Company Website"] = kk.CompanyWebsite;
                  
                    dt.Rows.Add(row);
                }
                Byte[] fileBytes = null;

                if (customerList != null)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("LoanCustomer");
                        ws.DefaultColWidth = 20;
                        ws.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                        fileBytes = pck.GetAsByteArray();
                    }
                }
                return fileBytes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<DocumentTypeObj> GetAllDocumentType()
        {
            try
            {
                var documentType = (from a in _context.credit_documenttype
                                    where a.Deleted == false
                                    select new DocumentTypeObj
                                    {
                                        DocumentTypeId = a.DocumentTypeId,
                                        DocumentTypeName = a.Name
                                    }).ToList();

                return documentType;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<LoanCustomerObj>> GetLoanCustomersAsync()
        {
            return await (from a in _context.credit_loancustomer
                   where a.Deleted == false
                   select

                   new LoanCustomerObj
                   {
                       CustomerId = a.CustomerId,
                       CustomerTypeId = a.CustomerTypeId,
                       CustomerTypeName = a.CustomerTypeId == 1 ? "Individual" : "Corporate",
                       TitleId = a.TitleId,
                       //Title = a.cor_title.Title,
                       Address = a.Address,
                       AnnualTurnover = a.AnnualTurnover,
                       ApprovalStatusId = a.ApprovalStatusId,
                       FirstName = a.FirstName,
                       LastName = a.LastName,
                       MiddleName = a.MiddleName,
                       Dob = a.DOB,
                       PostalAddress = a.PostaAddress,
                       CityId = a.CityId,
                       City = a.City,
                       Occupation = a.Occupation,
                       EmploymentType = a.EmploymentType,
                       //Employment = a.cor_employertype.Type,
                       Email = a.Email,
                       CompanyName = a.CompanyName,
                       CompanyWebsite = a.CompanyWebsite,
                       PhoneNo = a.PhoneNo,
                       RegistrationNo = a.RegistrationNo,
                       PoliticallyExposed = a.PoliticallyExposed,
                       CountryId = a.CountryId,
                       //Country = a.cor_country.CountryName,
                       Industry = a.Industry,
                       IncorporationCountry = a.IncorporationCountry,
                       ShareholderFund = a.ShareholderFund,
                       //CustomerTypeName = a.cor_customertype.CustomerName,
                       GenderId = a.GenderId,
                       //Gender = a.cor_gender.Gender,
                       MaritalStatusId = a.MaritalStatusId,
                       //MaritalStatus = a.cor_maritalstatus.Status,
                       RelationshipOfficerId = a.RelationshipManagerId,
                       CASAAccountNumber = a.CASAAccountNumber,
                       //Director = _context.credit_loancustomerdirector.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerDirectorObj()
                       //{
                       //    CustomerDirectorId = x.CustomerDirectorId,
                       //    CustomerId = x.CustomerId,
                       //    Name = x.Name,
                       //    Position = x.Position,
                       //    PoliticallyPosition = x.PoliticallyPosition,
                       //    RelativePoliticallyPosition = x.RelativePoliticallyPosition,
                       //    Dob = x.DateOfBirth,
                       //    Email = x.Email,
                       //    Signature = x.Signature,
                       //    PhoneNo = x.PhoneNo,
                       //    Address = x.Address,

                       //}).ToList(),

                       //Shareholder = _context.credit_directorshareholder.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerDirectorShareHolderObj()
                       //{
                       //    CustomerId = x.CustomerId,
                       //    DirectorShareHolderId = x.DirectorShareHolderId,
                       //    CompanyName = x.CompanyName,
                       //    PercentageHolder = x.PercentageHolder,
                       //}).ToList(),

                       //Bank = _context.credit_customerbankdetails.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerBankDetailsObj()
                       //{
                       //    CustomerBankDetailsId = x.CustomerBankDetailsId,
                       //    CustomerId = x.CustomerId,
                       //    Bvn = x.BVN,
                       //    Account = x.Account,
                       //    Bank = x.Bank,

                       //}).ToList(),

                       //Nextofkin = _context.credit_customernextofkin.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerNextOfKinObj()
                       //{
                       //    CustomerNextOfKinId = x.CustomerNextOfKinId,
                       //    CustomerId = x.CustomerId,
                       //    Name = x.Name,
                       //    Relationship = x.Relationship,
                       //    Email = x.Email,
                       //    PhoneNo = x.PhoneNo,
                       //    Address = x.Address,

                       //}).ToList(),

                       //Document = _context.credit_loancustomerdocument.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerDocumentObj()
                       //{
                       //    CustomerDocumentId = x.CustomerDocumentId,
                       //    CustomerId = x.CustomerId,
                       //    DocumentExtension = x.DocumentExtension,
                       //    DocumentName = x.DocumentName,
                       //    PhysicalLocation = x.PhysicalLocation,

                       //}).ToList(),
                   }).ToListAsync();
        }

        private IQueryable<LoanCustomerObj> GetLoanCustomers(int customerId)
        {
            return from a in _context.credit_loancustomer
                   where a.Deleted == false && a.CustomerId == customerId
                   select

                   new LoanCustomerObj
                   {
                       CustomerId = a.CustomerId,
                       CustomerTypeId = a.CustomerTypeId,
                       TitleId = a.TitleId,
                       //Title = a.cor_title.Title,
                       Address = a.Address,
                       AnnualTurnover = a.AnnualTurnover,
                       ApprovalStatusId = a.ApprovalStatusId,
                       FirstName = a.FirstName,
                       LastName = a.LastName,
                       MiddleName = a.MiddleName,
                       Dob = a.DOB,
                       PostalAddress = a.PostaAddress,
                       CityId = a.CityId,
                       City = a.City,
                       Occupation = a.Occupation,
                       EmploymentType = a.EmploymentType,
                       //Employment = a.cor_employertype.Type,
                       Email = a.Email,
                       CompanyName = a.CompanyName,
                       CompanyWebsite = a.CompanyWebsite,
                       PhoneNo = a.PhoneNo,
                       RegistrationNo = a.RegistrationNo,
                       PoliticallyExposed = a.PoliticallyExposed,
                       CountryId = a.CountryId,
                       //Country = a.cor_country.CountryName,
                       Industry = a.Industry,
                       IncorporationCountry = a.IncorporationCountry,
                       ShareholderFund = a.ShareholderFund,
                       //CustomerTypeName = a.cor_customertype.CustomerName,
                       GenderId = a.GenderId,
                       //Gender = a.cor_gender.Gender,
                       MaritalStatusId = a.MaritalStatusId,
                       //MaritalStatus = a.cor_maritalstatus.Status,
                       RelationshipOfficerId = a.RelationshipManagerId,
                       CASAAccountNumber = a.CASAAccountNumber,
                       //Director = _context.credit_loancustomerdirector.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerDirectorObj()
                       //{
                       //    CustomerDirectorId = x.CustomerDirectorId,
                       //    CustomerId = x.CustomerId,
                       //    Name = x.Name,
                       //    Position = x.Position,
                       //    PoliticallyPosition = x.PoliticallyPosition,
                       //    RelativePoliticallyPosition = x.RelativePoliticallyPosition,
                       //    Dob = x.DateOfBirth,
                       //    Email = x.Email,
                       //    Signature = x.Signature,
                       //    PhoneNo = x.PhoneNo,
                       //    Address = x.Address,

                       //}).ToList(),

                       //Shareholder = _context.credit_directorshareholder.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerDirectorShareHolderObj()
                       //{
                       //    CustomerId = x.CustomerId,
                       //    DirectorShareHolderId = x.DirectorShareHolderId,
                       //    CompanyName = x.CompanyName,
                       //    PercentageHolder = x.PercentageHolder,
                       //}).ToList(),

                       //Bank = _context.credit_customerbankdetails.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerBankDetailsObj()
                       //{
                       //    CustomerBankDetailsId = x.CustomerBankDetailsId,
                       //    CustomerId = x.CustomerId,
                       //    Bvn = x.BVN,
                       //    Account = x.Account,
                       //    Bank = x.Bank,

                       //}).ToList(),

                       //Nextofkin = _context.credit_customernextofkin.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerNextOfKinObj()
                       //{
                       //    CustomerNextOfKinId = x.CustomerNextOfKinId,
                       //    CustomerId = x.CustomerId,
                       //    Name = x.Name,
                       //    Relationship = x.Relationship,
                       //    Email = x.Email,
                       //    PhoneNo = x.PhoneNo,
                       //    Address = x.Address,

                       //}).ToList(),

                       //Document = _context.credit_loancustomerdocument.Where(x => x.CustomerId == a.CustomerId).Select(x => new LoanCustomerDocumentObj()
                       //{
                       //    CustomerDocumentId = x.CustomerDocumentId,
                       //    CustomerId = x.CustomerId,
                       //    DocumentExtension = x.DocumentExtension,
                       //    DocumentName = x.DocumentName,
                       //    PhysicalLocation = x.PhysicalLocation,

                       //}).ToList(),
                   };
        }

        public IEnumerable<LoanCustomerObj> GetAllLoanCustomer()
        {
            try
            {
                var loanCustomer = GetLoanCustomersAsync().Result;

                return loanCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<credit_loancustomer> GetCustomerByEmailAsync(string email)
        {
            var data = await _context.credit_loancustomer.FirstOrDefaultAsync(d => d.Deleted == false && d.Email.Trim().ToLower() == email.Trim().ToLower());
            return data;
        }

        public IEnumerable<LoanCustomerBankDetailsObj> GetAllLoanCustomerBankDetails()
        {
            try
            {
                var loanCustomerBankDetails = (from a in _context.credit_customerbankdetails
                                               where a.Deleted == false
                                               select new LoanCustomerBankDetailsObj
                                               {
                                                   Bank = a.Bank,
                                                   Bvn = a.BVN,
                                                   Account = a.Account,
                                                   CustomerId = a.CustomerId,
                                                   CustomerBankDetailsId = a.CustomerBankDetailsId
                                               }).ToList();

                return loanCustomerBankDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerDirectorObj> GetAllLoanCustomerDirector()
        {
            try
            {
                var loanCustomerDirector = (from a in _context.credit_loancustomerdirector
                                            where a.Deleted == false
                                            select new LoanCustomerDirectorObj
                                            {
                                                DirectorTypeId = a.DirectorTypeId,
                                                DirectorType = a.credit_directortype.Name,
                                                PercentageShare = a.PercentageShare,
                                                Name = a.Name,
                                                Position = a.Position,
                                                PoliticallyPosition = a.PoliticallyPosition,
                                                RelativePoliticallyPosition = a.RelativePoliticallyPosition,
                                                PhoneNo = a.PhoneNo,
                                                Email = a.Email,
                                                Signature = a.Signature,
                                                Dob = a.DateOfBirth,
                                                CustomerId = a.CustomerId,
                                                Address = a.Address,
                                                CustomerDirectorId = a.CustomerDirectorId
                                            }).ToList();

                return loanCustomerDirector;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerDirectorShareHolderObj> GetAllLoanCustomerDirectorShareHolder()
        {
            try
            {
                var loanCustomerDirectorShareHolder = (from a in _context.credit_directorshareholder
                                                       where a.Deleted == false
                                                       select new LoanCustomerDirectorShareHolderObj
                                                       {
                                                           CompanyName = a.CompanyName,
                                                           PercentageHolder = a.PercentageHolder,
                                                           DirectorShareHolderId = a.DirectorShareHolderId,
                                                           CustomerId = a.CustomerId,
                                                       }).ToList();

                return loanCustomerDirectorShareHolder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerDocumentObj> GetAllLoanCustomerDocument()
        {
            try
            {
                var loanCustomerDocument = (from a in _context.credit_loancustomerdocument
                                            where a.Deleted == false
                                            select new LoanCustomerDocumentObj
                                            {
                                                DocumentExtension = a.DocumentExtension,
                                                DocumentName = a.DocumentName,
                                                PhysicalLocation = a.PhysicalLocation,
                                                CustomerDocumentId = a.CustomerDocumentId,
                                                CustomerId = a.CustomerId,
                                            }).ToList();

                return loanCustomerDocument;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerIdentityDetailsObj> GetAllLoanCustomerIdentityDetails()
        {
            try
            {
                var loanCustomerIdentityDetails = (from a in _context.credit_customeridentitydetails
                                                   where a.Deleted == false
                                                   select new LoanCustomerIdentityDetailsObj
                                                   {
                                                       Number = a.Number,
                                                       Issuer = a.Issuer,
                                                       IdentificationId = a.IdentificationId,
                                                       //Identification = a.cor_identification.IdentificationName,
                                                       CustomerId = a.CustomerId,
                                                       CustomerIdentityDetailsId = a.CustomerIdentityDetailsId
                                                   }).ToList();

                return loanCustomerIdentityDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerLiteObj> GetAllLoanCustomerLite()
        {
            try
            {
                var loanCustomer = (from a in _context.credit_loancustomer
                                    where a.Deleted == false
                                    select
                                   new LoanCustomerLiteObj
                                   {
                                       CustomerId = a.CustomerId,
                                       FirstName = a.FirstName,
                                       LastName = a.LastName,
                                       Email = a.Email,
                                       PhoneNo = a.PhoneNo,
                                       AccountNumber = a.CASAAccountNumber,
                                       CustomerTypeName = a.CustomerTypeId == 1 ? "Individual" : "Corporate",
                                   }).ToList();

                return loanCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerLiteObj> GetCustomerLiteBySearch(string fullname, string email, string acctName)
        {
            IEnumerable<LoanCustomerLiteObj> data = null;
            data = (from a in _context.credit_loancustomer
                    where a.Deleted == false
                    select new LoanCustomerLiteObj
                    {
                        CustomerId = a.CustomerId,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Email = a.Email,
                        PhoneNo = a.PhoneNo,
                        AccountNumber = a.CASAAccountNumber,
                        CustomerTypeName = a.CustomerTypeId == 1 ? "Individual" : "Corporate",
                    }).AsQueryable();

            if (fullname != null && fullname != string.Empty)
            {
                data = data.Where(d => d.FirstName.ToLower().Trim().Contains(fullname.ToLower().Trim()));
            }
            if (email != null && email != string.Empty)
            {
                data = data.Where(d => d.Email.ToLower().Trim().Contains(email.ToLower().Trim()));
            }
            if (acctName != null && acctName != string.Empty)
            {
                data = data.Where(d => d.AccountNumber.Contains(acctName));
            }
            return data.ToList();
        }

        public IEnumerable<LoanCustomerNextOfKinObj> GetAllLoanCustomerNextOfKin()
        {
            try
            {
                var loanCustomerNextOfKin = (from a in _context.credit_customernextofkin
                                             where a.Deleted == false
                                             select new LoanCustomerNextOfKinObj
                                             {
                                                 Name = a.Name,
                                                 Relationship = a.Relationship,
                                                 PhoneNo = a.PhoneNo,
                                                 Email = a.Email,
                                                 CustomerNextOfKinId = a.CustomerNextOfKinId,
                                                 CustomerId = a.CustomerId,
                                                 Address = a.Address,
                                             }).ToList();

                return loanCustomerNextOfKin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerDirectorObj GetDirectorSignature(int DirectorId, int customerId)
        {
            try
            {
                var loanCustomerDirector = (from a in _context.credit_loancustomerdirector
                                            where a.Deleted == false && a.CustomerDirectorId == DirectorId && a.CustomerId == customerId
                                            select new LoanCustomerDirectorObj
                                            {
                                                DirectorTypeId = a.DirectorTypeId,
                                                DirectorType = a.credit_directortype.Name,
                                                PercentageShare = a.PercentageShare,
                                                Name = a.Name,
                                                Position = a.Position,
                                                PoliticallyPosition = a.PoliticallyPosition,
                                                RelativePoliticallyPosition = a.RelativePoliticallyPosition,
                                                PhoneNo = a.PhoneNo,
                                                Email = a.Email,
                                                Signature = a.Signature,
                                                Dob = a.DateOfBirth,
                                                CustomerId = a.CustomerId,
                                                Address = a.Address,
                                                CustomerDirectorId = a.CustomerDirectorId

                                            }).FirstOrDefault();

                return loanCustomerDirector;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerObj GetLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomer = GetLoanCustomers(loanCustomerId).Where(a => a.CustomerId == loanCustomerId).FirstOrDefault();

                return loanCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerBankDetailsObj GetLoanCustomerBankDetails(int loanCustomerBankDetailsId)
        {
            try
            {
                var loanCustomerBankDetails = (from a in _context.credit_customerbankdetails
                                               where a.Deleted == false && a.CustomerBankDetailsId == loanCustomerBankDetailsId
                                               select new LoanCustomerBankDetailsObj
                                               {
                                                   Account = a.Account,
                                                   Bank = a.Bank,
                                                   Bvn = a.BVN,
                                                   CustomerId = a.CustomerId,
                                                   CustomerBankDetailsId = a.CustomerBankDetailsId
                                               }).FirstOrDefault();

                return loanCustomerBankDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerBankDetailsObj GetLoanCustomerBankDetailsByCustomer(int customerId)
        {
            try
            {
                var loanCustomerBankDetails = (from a in _context.credit_customerbankdetails
                                               where a.Deleted == false && a.CustomerId == customerId
                                               select new LoanCustomerBankDetailsObj
                                               {
                                                   Account = a.Account,
                                                   Bank = a.Bank,
                                                   Bvn = a.BVN,
                                                   CustomerId = a.CustomerId,
                                                   CustomerBankDetailsId = a.CustomerBankDetailsId
                                               }).FirstOrDefault();

                return loanCustomerBankDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerBankDetailsObj> GetLoanCustomerBankDetailsByLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomerBankDetails = (from a in _context.credit_customerbankdetails
                                               where a.Deleted == false && a.CustomerId == loanCustomerId
                                               select new LoanCustomerBankDetailsObj
                                               {
                                                   Account = a.Account,
                                                   Bank = a.Bank,
                                                   Bvn = a.BVN,
                                                   CustomerId = a.CustomerId,
                                                   CustomerBankDetailsId = a.CustomerBankDetailsId,

                                               }).ToList();

                return loanCustomerBankDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerCardDetailsObj GetLoanCustomerCardDetailsByLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomerBankDetails = (from a in _context.credit_customercarddetails
                                               where a.Deleted == false && a.CustomerId == loanCustomerId
                                               select new LoanCustomerCardDetailsObj
                                               {
                                                   CardNumber = a.CardNumber,
                                                   Cvv = a.Cvv,
                                                   ExpiryMonth = a.ExpiryMonth,
                                                   ExpiryYear = a.ExpiryYear,
                                                   CustomerId = a.CustomerId,
                                                   CustomerCardDetailsId = a.CustomerCardDetailsId,
                                                   CurrencyCode = a.currencyCode
                                               }).FirstOrDefault();

                return loanCustomerBankDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerObj> GetLoanCustomerCASA(int customerId)
        {
            try
            {
                var customerCASA = (from a in _context.credit_loancustomer
                                    where a.Deleted == false && a.CustomerId == customerId
                                    select new LoanCustomerObj
                                    {
                                        CustomerId = a.CustomerId,
                                        FirstName = a.FirstName,
                                        LastName = a.LastName,
                                        Email = a.Email,
                                        PhoneNo = a.PhoneNo,
                                        CASAAccountNumber = a.CASAAccountNumber,
                                    }).ToList();
                return customerCASA;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerDirectorObj GetLoanCustomerDirector(int loanCustomerDirectorId)
        {
            try
            {
                var loanCustomerDirector = (from a in _context.credit_loancustomerdirector
                                            where a.Deleted == false && a.CustomerDirectorId == loanCustomerDirectorId
                                            select new LoanCustomerDirectorObj
                                            {
                                                DirectorTypeId = a.DirectorTypeId,
                                                DirectorType = a.credit_directortype.Name,
                                                PercentageShare = a.PercentageShare,
                                                Name = a.Name,
                                                Position = a.Position,
                                                PoliticallyPosition = a.PoliticallyPosition,
                                                RelativePoliticallyPosition = a.RelativePoliticallyPosition,
                                                PhoneNo = a.PhoneNo,
                                                Email = a.Email,
                                                Signature = a.Signature,
                                                Dob = a.DateOfBirth,
                                                CustomerId = a.CustomerId,
                                                Address = a.Address,
                                                CustomerDirectorId = a.CustomerDirectorId

                                            }).FirstOrDefault();

                return loanCustomerDirector;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerDirectorObj GetLoanCustomerDirectorByCustomer(int customerId)
        {
            try
            {
                var loanCustomerDirector = (from a in _context.credit_loancustomerdirector
                                            where a.Deleted == false && a.CustomerId == customerId
                                            select new LoanCustomerDirectorObj
                                            {
                                                DirectorTypeId = a.DirectorTypeId,
                                                DirectorType = a.credit_directortype.Name,
                                                PercentageShare = a.PercentageShare,
                                                Name = a.Name,
                                                Position = a.Position,
                                                PoliticallyPosition = a.PoliticallyPosition,
                                                RelativePoliticallyPosition = a.RelativePoliticallyPosition,
                                                PhoneNo = a.PhoneNo,
                                                Email = a.Email,
                                                Signature = a.Signature,
                                                Dob = a.DateOfBirth,
                                                CustomerId = a.CustomerId,
                                                Address = a.Address,
                                                CustomerDirectorId = a.CustomerDirectorId

                                            }).FirstOrDefault();

                return loanCustomerDirector;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerDirectorObj> GetLoanCustomerDirectorByLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomerDirector = (from a in _context.credit_loancustomerdirector
                                            where a.Deleted == false && a.CustomerId == loanCustomerId
                                            select new LoanCustomerDirectorObj
                                            {
                                                DirectorTypeId = a.DirectorTypeId,
                                                DirectorType = a.DirectorTypeId == 1 ? "Director" : a.DirectorTypeId == 2 ? "Shareholder" : a.DirectorTypeId == 3 ? "Director/Shareholder" : null,
                                                PercentageShare = a.PercentageShare,
                                                Name = a.Name,
                                                Position = a.Position,
                                                PoliticallyPosition = a.PoliticallyPosition,
                                                RelativePoliticallyPosition = a.RelativePoliticallyPosition,
                                                PhoneNo = a.PhoneNo,
                                                Email = a.Email,
                                                Signature = a.Signature,
                                                Dob = a.DateOfBirth,
                                                CustomerId = a.CustomerId,
                                                Address = a.Address,
                                                CustomerDirectorId = a.CustomerDirectorId

                                            }).ToList();

                return loanCustomerDirector;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerDirectorShareHolderObj GetLoanCustomerDirectorShareHolder(int loanCustomerDirectorShareHolderId)
        {
            try
            {
                var loanCustomerDirectorShareHolder = (from a in _context.credit_directorshareholder
                                                       where a.Deleted == false && a.DirectorShareHolderId == loanCustomerDirectorShareHolderId
                                                       select new LoanCustomerDirectorShareHolderObj
                                                       {
                                                           CompanyName = a.CompanyName,
                                                           PercentageHolder = a.PercentageHolder,
                                                           DirectorShareHolderId = a.DirectorShareHolderId,
                                                           CustomerId = a.CustomerId,

                                                       }).FirstOrDefault();

                return loanCustomerDirectorShareHolder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerDirectorShareHolderObj GetLoanCustomerDirectorShareHolderByCustomer(int customerId)
        {
            try
            {
                var loanCustomerDirectorShareHolder = (from a in _context.credit_directorshareholder
                                                       where a.Deleted == false && a.CustomerId == customerId
                                                       select new LoanCustomerDirectorShareHolderObj
                                                       {
                                                           CompanyName = a.CompanyName,
                                                           PercentageHolder = a.PercentageHolder,
                                                           DirectorShareHolderId = a.DirectorShareHolderId,
                                                           CustomerId = a.CustomerId,

                                                       }).FirstOrDefault();

                return loanCustomerDirectorShareHolder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerDirectorShareHolderObj> GetLoanCustomerDirectorShareHolderByLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomerDirectorShareHolder = (from a in _context.credit_directorshareholder
                                                       where a.Deleted == false && a.CustomerId == loanCustomerId
                                                       select new LoanCustomerDirectorShareHolderObj
                                                       {
                                                           CompanyName = a.CompanyName,
                                                           PercentageHolder = a.PercentageHolder,
                                                           DirectorShareHolderId = a.DirectorShareHolderId,
                                                           CustomerId = a.CustomerId,
                                                       }).ToList();

                return loanCustomerDirectorShareHolder;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerDocumentObj GetLoanCustomerDocument(int loanCustomerDocumentId)
        {
            try
            {
                var loanCustomerDocument = (from a in _context.credit_loancustomerdocument
                                            where a.Deleted == false && a.CustomerDocumentId == loanCustomerDocumentId
                                            select new LoanCustomerDocumentObj
                                            {
                                                DocumentExtension = a.DocumentExtension,
                                                DocumentName = a.DocumentName,
                                                DocumentFile = a.DocumentFile,
                                                PhysicalLocation = a.PhysicalLocation,
                                                CustomerDocumentId = a.CustomerDocumentId,
                                                CustomerId = a.CustomerId,

                                            }).FirstOrDefault();

                return loanCustomerDocument;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerDocumentObj GetLoanCustomerDocumentByCustomer(int customerId)
        {
            try
            {
                var loanCustomerDocument = (from a in _context.credit_loancustomerdocument
                                            where a.Deleted == false && a.CustomerId == customerId
                                            select new LoanCustomerDocumentObj
                                            {
                                                DocumentExtension = a.DocumentExtension,
                                                DocumentName = a.DocumentName,
                                                PhysicalLocation = a.PhysicalLocation,
                                                CustomerDocumentId = a.CustomerDocumentId,
                                                CustomerId = a.CustomerId,

                                            }).FirstOrDefault();

                return loanCustomerDocument;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerDocumentObj> GetLoanCustomerDocumentByLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomerDocument = (from a in _context.credit_loancustomerdocument
                                            where a.Deleted == false && a.CustomerId == loanCustomerId
                                            select new LoanCustomerDocumentObj
                                            {
                                                DocumentExtension = a.DocumentExtension,
                                                DocumentName = a.DocumentName,
                                                PhysicalLocation = a.PhysicalLocation,
                                                CustomerDocumentId = a.CustomerDocumentId,
                                                CustomerId = a.CustomerId,
                                            }).ToList();

                return loanCustomerDocument;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerIdentityDetailsObj GetLoanCustomerIdentityDetails(int loanCustomerIdentityDetailsId)
        {
            try
            {
                
                var loanCustomerIdentityDetails = (from a in _context.credit_customeridentitydetails
                                                   where a.Deleted == false && a.CustomerIdentityDetailsId == loanCustomerIdentityDetailsId
                                                   select new LoanCustomerIdentityDetailsObj
                                                   {
                                                       Number = a.Number,
                                                       Issuer = a.Issuer,
                                                       IdentificationId = a.IdentificationId,
                                                       CustomerId = a.CustomerId,
                                                       CustomerIdentityDetailsId = a.CustomerIdentityDetailsId,
                                                   }).FirstOrDefault();
                
                return loanCustomerIdentityDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerIdentityDetailsObj GetLoanCustomerIdentityDetailsByCustomer(int customerId)
        {
            try
            {
                var identityTypeList = _identityServer.GetIdentiticationTypeAsync().Result;
                var loanCustomerIdentityDetails = (from a in _context.credit_customeridentitydetails
                                                   where a.Deleted == false && a.CustomerId == customerId
                                                   select new LoanCustomerIdentityDetailsObj
                                                   {
                                                       Number = a.Number,
                                                       Issuer = a.Issuer,
                                                       IdentificationId = a.IdentificationId,
                                                       CustomerId = a.CustomerId,
                                                       CustomerIdentityDetailsId = a.CustomerIdentityDetailsId,
                                                       Identification = string.Empty,
                                                   }).FirstOrDefault();
                if (loanCustomerIdentityDetails != null)
                {
                    loanCustomerIdentityDetails.Identification = identityTypeList.commonLookups.FirstOrDefault(x => x.LookupId == loanCustomerIdentityDetails.IdentificationId)?.LookupName;
                }

                return loanCustomerIdentityDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerIdentityDetailsObj> GetLoanCustomerIdentityDetailsByLoanCustomer(int loanCustomerId)
        {
            try
            {
                var identityTypeList = _identityServer.GetIdentiticationTypeAsync().Result;
                var loanCustomerIdentityDetails = (from a in _context.credit_customeridentitydetails
                                                   where a.Deleted == false && a.CustomerId == loanCustomerId
                                                   select new LoanCustomerIdentityDetailsObj
                                                   {
                                                       Number = a.Number,
                                                       Issuer = a.Issuer,
                                                       Identification = string.Empty,
                                                       IdentificationId = a.IdentificationId,
                                                       CustomerId = a.CustomerId,
                                                       CustomerIdentityDetailsId = a.CustomerIdentityDetailsId,
                                                   }).ToList();
               foreach (var item in loanCustomerIdentityDetails)
                {
                    item.Identification = identityTypeList.commonLookups.FirstOrDefault(x => x.LookupId == item.IdentificationId)?.LookupName;
                }
                return loanCustomerIdentityDetails;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public LoanCustomerNextOfKinObj GetLoanCustomerNextOfKin(int loanCustomerNextOfKinId)
        {
            try
            {
                var loanCustomerNextOfKin = (from a in _context.credit_customernextofkin
                                             where a.Deleted == false && a.CustomerNextOfKinId == loanCustomerNextOfKinId
                                             select new LoanCustomerNextOfKinObj
                                             {
                                                 Name = a.Name,
                                                 Relationship = a.Relationship,
                                                 PhoneNo = a.PhoneNo,
                                                 Email = a.Email,
                                                 CustomerNextOfKinId = a.CustomerNextOfKinId,
                                                 CustomerId = a.CustomerId,
                                                 Address = a.Address,

                                             }).FirstOrDefault();

                return loanCustomerNextOfKin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerNextOfKinObj GetLoanCustomerNextOfKinByCustomer(int customerId)
        {
            try
            {
                var loanCustomerNextOfKin = (from a in _context.credit_customernextofkin
                                             where a.Deleted == false && a.CustomerId == customerId
                                             select new LoanCustomerNextOfKinObj
                                             {
                                                 Name = a.Name,
                                                 Relationship = a.Relationship,
                                                 PhoneNo = a.PhoneNo,
                                                 Email = a.Email,
                                                 CustomerNextOfKinId = a.CustomerNextOfKinId,
                                                 CustomerId = a.CustomerId,
                                                 Address = a.Address,

                                             }).FirstOrDefault();

                return loanCustomerNextOfKin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<LoanCustomerNextOfKinObj> GetLoanCustomerNextOfKinByLoanCustomer(int loanCustomerId)
        {
            try
            {
                var loanCustomerNextOfKin = (from a in _context.credit_customernextofkin
                                             where a.Deleted == false && a.CustomerId == loanCustomerId
                                             select new LoanCustomerNextOfKinObj
                                             {
                                                 Name = a.Name,
                                                 Relationship = a.Relationship,
                                                 PhoneNo = a.PhoneNo,
                                                 Email = a.Email,
                                                 CustomerNextOfKinId = a.CustomerNextOfKinId,
                                                 CustomerId = a.CustomerId,
                                                 Address = a.Address,

                                             }).ToList();

                return loanCustomerNextOfKin;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<StartLoanApplicationCustomerObj> GetStartLoanApplicationCustomer()
        {
            try
            {
                var customerList = _context.credit_loancustomer.Where(x => x.Deleted == false).ToList();
                var loanList = _context.credit_loan.Where(x => x.Deleted == false).ToList();

                var loanCustomer = customerList.Select(a => new StartLoanApplicationCustomerObj
                                   {
                                       CustomerId = a.CustomerId,
                                       FirstName = a.FirstName,
                                       LastName = a.LastName,
                                       Email = a.Email,
                                       PhoneNo = a.PhoneNo,
                                       CustomerTypeName = a.CustomerTypeId == 1 ? "Individual" : "Corporate",
                                       CurrentExposure = loanList.Where(x => x.CustomerId == a.CustomerId).Sum(x => x.OutstandingPrincipal),
                                       TotalExposure = _context.credit_exposureparament.FirstOrDefault(x => x.CustomerTypeId == a.CustomerTypeId)?.Amount,
                                   }).ToList();

                return loanCustomer.GroupBy(k => k.CustomerId).Select(l => l.FirstOrDefault());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<StartLoanApplicationCustomerObj> GetStartLoanCustomerBySearch(string fullname, string email, string acctName)
        {
            var customerList = _context.credit_loancustomer.Where(x => x.Deleted == false).ToList();
            var loanList = _context.credit_loan.Where(x => x.Deleted == false).ToList();

            var data = customerList.Select(a => new StartLoanApplicationCustomerObj
            {
                CustomerId = a.CustomerId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                PhoneNo = a.PhoneNo,
                AccountNumber = a.CASAAccountNumber,
                CustomerTypeName = a.CustomerTypeId == 1 ? "Individual" : "Corporate",
                CurrentExposure = loanList.Where(x => x.CustomerId == a.CustomerId).Sum(x => x.OutstandingPrincipal),
                TotalExposure = _context.credit_exposureparament.FirstOrDefault(x => x.CustomerTypeId == a.CustomerTypeId)?.Amount,
            }).AsQueryable();

            if (fullname != null && fullname != string.Empty)
            {
                data = data.Where(d => d.FirstName.ToLower().Trim().Contains(fullname.ToLower().Trim()));
            }
            if (email != null && email != string.Empty)
            {
                data = data.Where(d => d.Email.ToLower().Trim().Contains(email.ToLower().Trim()));
            }
            if (acctName != null && acctName != string.Empty)
            {
                data = data.Where(d => d.AccountNumber.Contains(acctName));
            }
            return data.GroupBy(k => k.CustomerId).Select(l => l.FirstOrDefault());
        }

        public IEnumerable<GLTransactionObj> GetWebCustomerTransactionDetails(string accountNumber)
        {
            try
            {
                var customerCASA = (from b in _context.fin_customertransaction
                                    where b.AccountNumber == accountNumber
                                    select new GLTransactionObj
                                    {
                                        TransactionDate = b.TransactionDate,
                                        CreditAmount = b.Amount ?? 0,
                                        Description = b.Description,
                                        TransactionType = b.TransactionType,
                                    }).ToList().OrderByDescending(x => x.TransactionDate).Take(5);
                return customerCASA;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public StartLoanApplicationCustomerObj GetStartLoanApplicationCustomerById(int id)
        {
            try
            {
                var loanCustomer = (from a in _context.credit_loancustomer
                                    join b in _context.credit_loan on a.CustomerId equals b.CustomerId
                                    into loan
                                    from b in loan.DefaultIfEmpty()
                                    where a.Deleted == false && a.CustomerId == id
                                    select

                                   new StartLoanApplicationCustomerObj
                                   {
                                       CustomerId = a.CustomerId,
                                       FirstName = a.FirstName,
                                       LastName = a.LastName,
                                       Email = a.Email,
                                       PhoneNo = a.PhoneNo,
                                       CustomerTypeName = a.CustomerTypeId == 1 ? "Individual" : "Corporate",
                                       CurrentExposure = loan.Sum(x => x.OutstandingPrincipal) > 0 ? loan.Sum(x => x.OutstandingPrincipal) : 0,
                                       TotalExposure = _context.credit_exposureparament.Where(x => x.CustomerTypeId == a.CustomerTypeId).FirstOrDefault().Amount,
                                   }).ToList();

                return loanCustomer.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerObj GetWebLoanCustomerCASA(int customerId)
        {
            try
            {
                var staffList = _identityServer.GetAllStaffAsync().Result;
                var customerCASA = (from a in _context.credit_loancustomer
                                    where a.Deleted == false && a.CustomerId == customerId
                                    select new LoanCustomerObj
                                    {
                                        InFlow = _context.fin_customertransaction.Where(x => x.AccountNumber == a.CASAAccountNumber).Select(x => x.CreditAmount).Sum(),
                                        OutFlow = _context.fin_customertransaction.Where(x => x.AccountNumber == a.CASAAccountNumber).Select(x => x.DebitAmount).Sum(),
                                        Balance = _context.credit_casa.Where(x => x.AccountNumber == a.CASAAccountNumber).FirstOrDefault().AvailableBalance,
                                        CASAAccountNumber = a.CASAAccountNumber,
                                        RelationshipOfficerId = a.RelationshipManagerId
                                    }).FirstOrDefault();
                customerCASA.RelationshipManager = staffList.staff.FirstOrDefault(x => x.staffId == customerCASA.RelationshipOfficerId)?.firstName + " " + staffList.staff.FirstOrDefault(x => x.staffId == customerCASA.RelationshipOfficerId)?.lastName;
                customerCASA.RelationshipManagerEmail = staffList.staff.FirstOrDefault(x => x.staffId == customerCASA.RelationshipOfficerId)?.email;
                return customerCASA;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoanCustomerObj> Login(string username, string password)
        {
            var user = (from a in _context.credit_loancustomer
                        where a.Email == username && a.ApprovalStatusId == 2
                        select new LoanCustomerObj
                        {
                            CountryId = a.CountryId,
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            Email = a.Email,
                            PhoneNo = a.PhoneNo,
                            CustomerId = a.CustomerId,
                            ProfileStatus = a.ProfileStatus,
                            CASAAccountNumber = a.CASAAccountNumber,
                            CustomerTypeId = a.CustomerTypeId,
                            CustomerTypeName = a.CustomerTypeId == 1 ? "Individual" : "Corporate",
                        }).FirstOrDefault();


            if (user == null)
                throw new Exception("User doesn't exist or has not been verified!");

            var cred = (from a in _context.credit_loancustomer
                        where a.Email == username
                        select a).FirstOrDefault();
            if (!VerifyPasswordHash(password, cred.PasswordHash, cred.PasswordSalt))
                throw new Exception("Password is Incorrect!");
            //return null;

            return user;
        }

        public bool ResetPassword(ResetPasswordViewModel entity)
        {
            //var resetPassword = _context.cor_resetpassword.Where(x => x.Guid == entity.guid && x.EndTime > DateTime.Now).FirstOrDefault();
            //if (resetPassword == null)
            //    throw new Exception("Reset Password Link has expired");
            //resetPassword.ResetPassword = true;
            //var account = (from a in _context.credit_loancustomer
            //               where a.CustomerId == resetPassword.UserAccountId
            //               select a).FirstOrDefault();
            //if (account != null)
            //{
            //    byte[] passwordHash, passwordSalt;
            //    CreatePasswordHash(entity.newPassword, out passwordHash, out passwordSalt);
            //    account.PasswordHash = passwordHash;
            //    account.PasswordSalt = passwordSalt;
            //}
            //return _context.SaveChanges() > 0;
            throw new NotImplementedException();
        }

        public LoanCustomerRespObj UpdateLoanCustomer(LoanCustomerObj entity)
        {
            try
            {
                using (var trans = _context.Database.BeginTransaction())
                {
                    if (entity == null) return null;
                credit_loancustomer loanCustomer = null;
                var accountNumber = GenerateRandomDigitCode(10);
                if (entity.CASAAccountNumber == null)
                {
                    entity.CASAAccountNumber = accountNumber;
                }
                if (entity.CustomerId > 0)
                {
                    loanCustomer = _context.credit_loancustomer.Find(entity.CustomerId);
                    if (loanCustomer != null)
                    {
                        loanCustomer.CustomerTypeId = entity.CustomerTypeId;
                        loanCustomer.TitleId = entity.TitleId;
                        loanCustomer.FirstName = entity.FirstName;
                        loanCustomer.LastName = entity.LastName;
                        loanCustomer.MiddleName = entity.MiddleName;
                        loanCustomer.GenderId = entity.GenderId;
                        loanCustomer.DOB = entity.Dob;
                        loanCustomer.Address = entity.Address;
                        loanCustomer.PostaAddress = entity.PostalAddress;
                        loanCustomer.Email = entity.Email;
                        loanCustomer.EmploymentType = entity.EmploymentType;
                        loanCustomer.IncorporationCountry = entity.IncorporationCountry;
                        loanCustomer.Industry = entity.Industry;
                        loanCustomer.ShareholderFund = entity.ShareholderFund;
                        loanCustomer.CityId = entity.CityId;
                        loanCustomer.City = entity.City;
                        loanCustomer.Occupation = entity.Occupation;
                        loanCustomer.PoliticallyExposed = entity.PoliticallyExposed;
                        loanCustomer.CompanyName = entity.CompanyName;
                        loanCustomer.CompanyWebsite = entity.CompanyWebsite;
                        loanCustomer.RegistrationNo = entity.RegistrationNo;
                        loanCustomer.CountryId = entity.CountryId;
                        loanCustomer.PhoneNo = entity.PhoneNo;
                        loanCustomer.MaritalStatusId = entity.MaritalStatusId;
                        loanCustomer.RelationshipManagerId = entity.RelationshipOfficerId;
                        loanCustomer.AnnualTurnover = entity.AnnualTurnover;
                        loanCustomer.ProfileStatus = entity.ProfileStatus;
                        //loanCustomer.ApprovalStatusId = entity.approvalStatusId;
                        loanCustomer.Active = true;
                        loanCustomer.Deleted = false;
                        loanCustomer.UpdatedBy = entity.CreatedBy;
                        loanCustomer.UpdatedOn = DateTime.Now;
                        var updateUser = UpdateUserAsync(loanCustomer).Result;
                    }
                }
                else
                {
                    var emailExist = EmailExists(entity.Email);
                    if (emailExist)
                    {
                        return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Email for this customer is already taken" } }
                        };
                    }
                    loanCustomer = new credit_loancustomer
                    {
                        CustomerTypeId = entity.CustomerTypeId,
                        TitleId = entity.TitleId,
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        MiddleName = entity.MiddleName,
                        GenderId = entity.GenderId,
                        Address = entity.Address,
                        DOB = entity.Dob,
                        RegistrationSource = "Application",
                        PostaAddress = entity.PostalAddress,
                        Email = entity.Email,
                        EmploymentType = entity.EmploymentType,
                        IncorporationCountry = entity.IncorporationCountry,
                        Industry = entity.Industry,
                        ShareholderFund = entity.ShareholderFund,
                        CityId = entity.CityId,
                        City = entity.City,
                        Occupation = entity.Occupation,
                        PoliticallyExposed = entity.PoliticallyExposed,
                        CompanyName = entity.CompanyName,
                        CompanyWebsite = entity.CompanyWebsite,
                        RegistrationNo = entity.RegistrationNo,
                        AnnualTurnover = entity.AnnualTurnover,
                        //ApprovalStatusId = entity.approvalStatusId,
                        CountryId = entity.CountryId,
                        PhoneNo = entity.PhoneNo,
                        MaritalStatusId = entity.MaritalStatusId,
                        RelationshipManagerId = entity.RelationshipOfficerId,
                        CASAAccountNumber = entity.CASAAccountNumber,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                        ProfileStatus = entity.ProfileStatus
                    };
                    _context.credit_loancustomer.Add(loanCustomer);
                
                    updateCASA(entity, loanCustomer.CustomerId, entity.CASAAccountNumber); 

                    var created = CreateAsUserAsync(loanCustomer).Result;
                        if (!created.Status.IsSuccessful)
                        {
                            return new LoanCustomerRespObj()
                            {
                                Status = new APIResponseStatus()
                                {
                                    Message = new APIResponseMessage
                                    {
                                        FriendlyMessage = created.Status.Message.FriendlyMessage
                                    }
                                }
                            };
                        }
                        loanCustomer.UserIdentity = created.UserId;
                }

                    try
                    {  
                        var response = _context.SaveChanges() > 0;

                            trans.Commit();
                            return new LoanCustomerRespObj()
                            {
                                CustomerId = loanCustomer.CustomerId,
                                Status = new APIResponseStatus
                                {
                                    IsSuccessful = true,
                                    Message = new APIResponseMessage
                                    {
                                        FriendlyMessage = "Successful"
                                    }
                                }
                            };
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }finally
                    {
                        trans.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<LoanCustomerRespObj> CreateAsUserAsync(credit_loancustomer loanCustomer)
        {
            var user = new ApplicationUser()
            {
                Address = loanCustomer.Address,
                UserName = loanCustomer.Email,
                Email = loanCustomer.Email,
                PhoneNumber = loanCustomer.PhoneNo,
                FirstName = loanCustomer.FirstName,
                LastName = loanCustomer.LastName,
                CustomerTypeId = loanCustomer.CustomerTypeId
            };
            var created =  await _userManager.CreateAsync(user, "Password@1");
            if (!created.Succeeded)
            {
                return new LoanCustomerRespObj()
                {
                    Status = new APIResponseStatus()
                    {
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = created.Errors.FirstOrDefault().Description
                        },
                        IsSuccessful = false
                    }
                };
            }
            return new LoanCustomerRespObj()
            {
                UserId = user.Id,
                Status = new APIResponseStatus()
                {
                    Message = new APIResponseMessage
                    {
                        FriendlyMessage = "Successful"
                    },
                    IsSuccessful = true
                }
            };
        }

        private async Task<bool> UpdateUserAsync(credit_loancustomer loanCustomer)
        {
            var user = await _userManager.FindByIdAsync(loanCustomer.UserIdentity);
            if(user != null)
            {
                user.Address = loanCustomer.Address;
                user.UserName = loanCustomer.Email;
                user.Email = loanCustomer.Email;
                user.PhoneNumber = loanCustomer.PhoneNo;
                user.FirstName = loanCustomer.FirstName;
                user.LastName = loanCustomer.LastName;
                user.CustomerTypeId = loanCustomer.CustomerTypeId;
                var er = await _userManager.UpdateAsync(user);
            }             
            return true;
        }
        public LoanCustomerIdentityRespObj UpdateLoanCustomerBankDetails(LoanCustomerBankDetailsObj entity)
        {
            try
            {
                var customer = _context.credit_loancustomer.Find(entity.CustomerId);
                if (customer == null)
                {
                    return new LoanCustomerIdentityRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Customer doesn't exist" } }
                    };
                }
                if (entity == null)
                    return new LoanCustomerIdentityRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid request payload" } }
                    };
                if (entity.Account != string.Empty)
                {
                    var flutter = _identityServer.GetFlutterWaveKeys().Result;
                    if (flutter.keys.useFlutterWave)
                    {
                    var bankcode = entity.Bank.Split("-");
                    var accountObj = new AccountObj
                    {
                        account_bank = bankcode[0],
                        account_number = entity.Account.Trim()
                    };                   
                    var response = _flutter.validateAccountDetails(accountObj).Result;
                    if (response.status != "success")
                    {
                        return new LoanCustomerIdentityRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = response.message } }
                        };
                    }
                    else
                    {
                        if(customer.CustomerTypeId == 1)//Individual customer
                        {
                            if (!response.data.account_name.ToLower().Contains(customer.FirstName.ToLower()) && !response.data.account_number.ToLower().Contains(customer.LastName.ToLower()))
                            {
                                return new LoanCustomerIdentityRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Account Name doesn't match this user" } } };
                            }
                            ///BVN Verification
                            if (entity.Bvn != string.Empty)
                            {
                                var url = "kyc/bvns/" + entity.Bvn.Trim();
                                var res = _flutter.validateBvnDetails(url).Result;
                                if (res.status != "success")
                                {
                                    return new LoanCustomerIdentityRespObj
                                    {
                                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = res.status } }
                                    };
                                }
                                else
                                {
                                    if (!res.data.first_name.ToLower().Contains(customer.FirstName.ToLower()) || !res.data.last_name.ToLower().Contains(customer.LastName.ToLower()))
                                    {
                                        return new LoanCustomerIdentityRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "BVN doesn't match this user" } } };
                                    }
                                }
                            }
                        }
                        else if(customer.CustomerTypeId == 2)//Corporate customer
                        {
                            if (!response.data.account_name.ToLower().Contains(customer.FirstName.ToLower()))
                            {
                                return new LoanCustomerIdentityRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Account Name doesn't match this user" } } };
                            }
                        }
                    }
                    }
                }

                var loanCustomerBankDetailsExist = _context.credit_customerbankdetails.FirstOrDefault(x=>x.Account == entity.Account && x.CustomerId == entity.CustomerId);
                    if (loanCustomerBankDetailsExist != null)
                    {
                        loanCustomerBankDetailsExist.BVN = entity.Bvn;
                        loanCustomerBankDetailsExist.Account = entity.Account;
                        loanCustomerBankDetailsExist.Bank = entity.Bank;
                        loanCustomerBankDetailsExist.CustomerId = entity.CustomerId;
                        loanCustomerBankDetailsExist.Active = true;
                        loanCustomerBankDetailsExist.Deleted = false;
                        loanCustomerBankDetailsExist.UpdatedBy = entity.CreatedBy;
                        loanCustomerBankDetailsExist.UpdatedOn = DateTime.Now;
                    }
                else
                {
                    var loanCustomerBankDetails = new credit_customerbankdetails
                    {
                        BVN = entity.Bvn,
                        Account = entity.Account,
                        Bank = entity.Bank,
                        CustomerId = entity.CustomerId,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = entity.CreatedBy,
                        UpdatedOn = DateTime.Now,
                    };
                    _context.credit_customerbankdetails.Add(loanCustomerBankDetails);
                }

                var isDone = _context.SaveChanges() > 0;

                return new LoanCustomerIdentityRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = isDone ? true : false, Message = new APIResponseMessage { FriendlyMessage = isDone ? "Successful" : "Unsuccessful" } }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LoanCustomerRespObj> UploadCustomerBankDetails(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                List<LoanCustomerBankDetailsObj> uploadedRecord = new List<LoanCustomerBankDetailsObj>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            uploadedRecord.Add(new LoanCustomerBankDetailsObj
                            {
                                CustomerEmail = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                Bvn = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                Account = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                                Bank = workSheet.Cells[i, 4].Value.ToString() != null ? workSheet.Cells[i, 4].Value.ToString() : "",
                                Bankcode = workSheet.Cells[i, 5].Value.ToString() != null ? workSheet.Cells[i, 5].Value.ToString() : "",
                            });
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    foreach (var entity in uploadedRecord)
                    {
                        if (entity.CustomerEmail == "" || entity.CustomerEmail == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid customer's email" } }
                        };
                        var accountTypeExist = _context.credit_loancustomer.Where(x => x.Email.ToLower().Trim() == entity.CustomerEmail.ToLower().Trim()).FirstOrDefault();
                        if (accountTypeExist == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = $" The customer email {entity.CustomerEmail} does not match any user" } }
                        };
                        if (entity.Bankcode == null || entity.Bankcode == "") return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = $" Please include a Bank Code" } }
                        };
                        if (entity.Bankcode == "044")//Access Bank
                        {
                            entity.Bank = "044-ACCESS";
                        }
                        else if (entity.Bankcode == "050")//Eco Bank
                        {
                            entity.Bank = "050-ECO-BANK";
                        }
                        else if (entity.Bankcode == "232")//Sterling Bank
                        {
                            entity.Bank = "232-STERLING";
                        }
                        else if (entity.Bankcode == "057")//Zenith Bank
                        {
                            entity.Bank = "057-ZENITH";
                        }
                        else if (entity.Bankcode == "214")//FCMB Bank
                        {
                            entity.Bank = "214-FIRST-CITY";
                        }
                        else if (entity.Bankcode == "076")//Polaris Bank
                        {
                            entity.Bank = "076-POLARIS";
                        }
                        else if (entity.Bankcode == "011")//First Bank
                        {
                            entity.Bank = "011-FIRST-BANK";
                        }
                        else if (entity.Bankcode == "070")//Fidelity Bank
                        {
                            entity.Bank = "070-FIDELITY";
                        }
                        else if (entity.Bankcode == "221")//Stanbic Bank
                        {
                            entity.Bank = "221-STANBIC";
                        }
                        else if (entity.Bankcode == "035")//Wema Bank
                        {
                            entity.Bank = "035-WEMA";
                        }
                        else if (entity.Bankcode == "221")//Stanbic Bank
                        {
                            entity.Bank = "221-STANBIC";
                        }
                        else if (entity.Bankcode == "063")//Diamond Bank
                        {
                            entity.Bank = "063-DIAMOND";
                        }
                        else if (entity.Bankcode == "058")//GTB Bank
                        {
                            entity.Bank = "058-GUARANTY-TRUST-BANK";
                        }
                        else if (entity.Bankcode == "032")//Diamond Bank
                        {
                            entity.Bank = "032-UNION";
                        }
                        else if (entity.Bankcode == "082")//Keystone Bank
                        {
                            entity.Bank = "082-KEYSTONE";
                        }
                        else if (entity.Bankcode == "033")//UBA Bank
                        {
                            entity.Bank = "033-UBA";
                        }
                        else
                        {
                            return new LoanCustomerRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = $" {entity.Bankcode} is not a valid bank code" } }
                            };
                        }

                        var loanCustomerBankDetailsExist = _context.credit_customerbankdetails.FirstOrDefault(x => x.Account == entity.Account && x.CustomerId == accountTypeExist.CustomerId && x.BVN == entity.Bvn);
                        if (loanCustomerBankDetailsExist != null)
                        {
                            loanCustomerBankDetailsExist.BVN = entity.Bvn;
                            loanCustomerBankDetailsExist.Account = entity.Account;
                            loanCustomerBankDetailsExist.Bank = entity.Bank;
                            loanCustomerBankDetailsExist.CustomerId = accountTypeExist.CustomerId;
                            loanCustomerBankDetailsExist.Active = true;
                            loanCustomerBankDetailsExist.Deleted = false;
                            loanCustomerBankDetailsExist.UpdatedBy = entity.CreatedBy;
                            loanCustomerBankDetailsExist.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            var loanCustomerBankDetails = new credit_customerbankdetails
                            {
                                BVN = entity.Bvn,
                                Account = entity.Account,
                                Bank = entity.Bank,
                                CustomerId = accountTypeExist.CustomerId,
                                Active = true,
                                Deleted = false,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = DateTime.Now,
                            };
                            _context.credit_customerbankdetails.Add(loanCustomerBankDetails);
                        }
                    }
                }
                var response = _context.SaveChanges() > 0;
                return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Records Uploaded Successfully" } }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerIdentityRespObj UpdateLoanCustomerCardDetails(LoanCustomerCardDetailsObj entity)
        {
            try
            {
                if (entity == null)
                    return new LoanCustomerIdentityRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Unsuccessful" } }
                    };
                if(entity.CardNumber != string.Empty)
                {
                    var flutter = _identityServer.GetFlutterWaveKeys().Result;
                    if (flutter.keys.useFlutterWave)
                    {
                        var cardBin = entity.CardNumber.Substring(0, 6);
                        var url = "card-bins/" + cardBin;
                        var response = _flutter.validateCardDetails(url).Result;
                        if (response.status.ToLower().Trim() != "success")
                        {
                            return new LoanCustomerIdentityRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Card Bin" } }
                            };
                        }
                        else
                        {
                            var bank = entity.IssuingBank.Split("-");
                            if (!response.data.issuer_info.Contains(bank[1]))
                            {
                                return new LoanCustomerIdentityRespObj
                                {
                                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Card bin does not match issuing bank" } }
                                };
                            }
                        }
                    }
                }

                var loanCustomerCardDetailsExist = _context.credit_customercarddetails.FirstOrDefault(x => entity.CustomerId == x.CustomerId && x.Deleted == false);
                if (loanCustomerCardDetailsExist != null)
                {
                        loanCustomerCardDetailsExist.CardNumber = entity.CardNumber;
                        loanCustomerCardDetailsExist.Cvv = entity.Cvv;
                        loanCustomerCardDetailsExist.ExpiryYear = entity.ExpiryYear;
                        loanCustomerCardDetailsExist.ExpiryMonth = entity.ExpiryMonth;
                        loanCustomerCardDetailsExist.currencyCode = entity.CurrencyCode;
                        loanCustomerCardDetailsExist.IssuingBank = entity.IssuingBank;
                        loanCustomerCardDetailsExist.CustomerId = entity.CustomerId;
                        loanCustomerCardDetailsExist.Active = true;
                        loanCustomerCardDetailsExist.Deleted = false;
                        loanCustomerCardDetailsExist.UpdatedBy = entity.CreatedBy;
                        loanCustomerCardDetailsExist.UpdatedOn = DateTime.Now;
                }
                else
                {
                    var loanCustomerCardDetails = new credit_customercarddetails
                    {
                        CardNumber = entity.CardNumber,
                        Cvv = entity.Cvv,
                        ExpiryMonth = entity.ExpiryMonth,
                        ExpiryYear = entity.ExpiryYear,
                        currencyCode = entity.CurrencyCode,
                        IssuingBank = entity.IssuingBank,
                        CustomerId = entity.CustomerId,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = entity.CreatedBy,
                        UpdatedOn = DateTime.Now,
                    };
                    _context.credit_customercarddetails.Add(loanCustomerCardDetails);
                }

                var isDone = _context.SaveChanges() > 0;

                return new LoanCustomerIdentityRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = isDone ? true : false, Message = new APIResponseMessage { FriendlyMessage = isDone ? "Successful" : "Unsuccessful" } }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LoanCustomerRespObj> UploadCustomerCardDetails(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                List<LoanCustomerCardDetailsObj> uploadedRecord = new List<LoanCustomerCardDetailsObj>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            uploadedRecord.Add(new LoanCustomerCardDetailsObj
                            {
                                CustomerEmail = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                CardNumber = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                Cvv = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                                ExpiryYear = workSheet.Cells[i, 4].Value.ToString() != null ? workSheet.Cells[i, 4].Value.ToString() : "",
                                ExpiryMonth = workSheet.Cells[i, 5].Value.ToString() != null ? workSheet.Cells[i, 5].Value.ToString() : "",
                                CurrencyCode = workSheet.Cells[i, 6].Value.ToString() != null ? workSheet.Cells[i, 6].Value.ToString() : "",
                                IssuingBank = workSheet.Cells[i, 7].Value.ToString() != null ? workSheet.Cells[i, 7].Value.ToString() : "",
                                Bankcode = workSheet.Cells[i, 8].Value.ToString() != null ? workSheet.Cells[i, 8].Value.ToString() : "",
                            });
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    foreach (var entity in uploadedRecord)
                    {
                        if (entity.CustomerEmail == "" || entity.CustomerEmail == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid customer's email" } }
                        };
                        var accountTypeExist = _context.credit_loancustomer.Where(x => x.Email.ToLower().Trim() == entity.CustomerEmail.ToLower().Trim()).FirstOrDefault();
                        if (accountTypeExist == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = $" The customer email {entity.CustomerEmail} does not match any user" } }
                        };
                        if (entity.Bankcode == null || entity.Bankcode == "") return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = $" Please include a Bank Code" } }
                        };
                        if (entity.Bankcode == "044")//Access Bank
                        {
                            entity.IssuingBank = "044-ACCESS";
                        }
                        else if (entity.Bankcode == "050")//Eco Bank
                        {
                            entity.IssuingBank = "050-ECO-BANK";
                        }
                        else if (entity.Bankcode == "232")//Sterling Bank
                        {
                            entity.IssuingBank = "232-STERLING";
                        }
                        else if (entity.Bankcode == "057")//Zenith Bank
                        {
                            entity.IssuingBank = "057-ZENITH";
                        }
                        else if (entity.Bankcode == "214")//FCMB Bank
                        {
                            entity.IssuingBank = "214-FIRST-CITY";
                        }
                        else if (entity.Bankcode == "076")//Polaris Bank
                        {
                            entity.IssuingBank = "076-POLARIS";
                        }
                        else if (entity.Bankcode == "011")//First Bank
                        {
                            entity.IssuingBank = "011-FIRST-BANK";
                        }
                        else if (entity.Bankcode == "070")//Fidelity Bank
                        {
                            entity.IssuingBank = "070-FIDELITY";
                        }
                        else if (entity.Bankcode == "221")//Stanbic Bank
                        {
                            entity.IssuingBank = "221-STANBIC";
                        }
                        else if (entity.Bankcode == "035")//Wema Bank
                        {
                            entity.IssuingBank = "035-WEMA";
                        }
                        else if (entity.Bankcode == "221")//Stanbic Bank
                        {
                            entity.IssuingBank = "221-STANBIC";
                        }
                        else if (entity.Bankcode == "063")//Diamond Bank
                        {
                            entity.IssuingBank = "063-DIAMOND";
                        }
                        else if (entity.Bankcode == "058")//GTB Bank
                        {
                            entity.IssuingBank = "058-GUARANTY-TRUST-BANK";
                        }
                        else if (entity.Bankcode == "032")//Diamond Bank
                        {
                            entity.IssuingBank = "032-UNION";
                        }
                        else if (entity.Bankcode == "082")//Keystone Bank
                        {
                            entity.IssuingBank = "082-KEYSTONE";
                        }
                        else if (entity.Bankcode == "033")//UBA Bank
                        {
                            entity.IssuingBank = "033-UBA";
                        }
                        else
                        {
                            return new LoanCustomerRespObj
                            {
                                Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = $" {entity.Bankcode} is not a valid bank code" } }
                            };
                        }

                        var loanCustomerCardDetailsExist = _context.credit_customercarddetails.FirstOrDefault(x => accountTypeExist.CustomerId == x.CustomerId && x.Deleted == false);
                        if (loanCustomerCardDetailsExist != null)
                        {
                            loanCustomerCardDetailsExist.CardNumber = entity.CardNumber;
                            loanCustomerCardDetailsExist.Cvv = entity.Cvv;
                            loanCustomerCardDetailsExist.ExpiryYear = entity.ExpiryYear;
                            loanCustomerCardDetailsExist.ExpiryMonth = entity.ExpiryMonth;
                            loanCustomerCardDetailsExist.currencyCode = entity.CurrencyCode;
                            loanCustomerCardDetailsExist.IssuingBank = entity.IssuingBank;
                            loanCustomerCardDetailsExist.CustomerId = accountTypeExist.CustomerId;
                            loanCustomerCardDetailsExist.Active = true;
                            loanCustomerCardDetailsExist.Deleted = false;
                            loanCustomerCardDetailsExist.UpdatedBy = entity.CreatedBy;
                            loanCustomerCardDetailsExist.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            var loanCustomerCardDetails = new credit_customercarddetails
                            {
                                CardNumber = entity.CardNumber,
                                Cvv = entity.Cvv,
                                ExpiryMonth = entity.ExpiryMonth,
                                ExpiryYear = entity.ExpiryYear,
                                currencyCode = entity.CurrencyCode,
                                IssuingBank = entity.IssuingBank,
                                CustomerId = accountTypeExist.CustomerId,
                                Active = true,
                                Deleted = false,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = DateTime.Now,
                                UpdatedBy = entity.CreatedBy,
                                UpdatedOn = DateTime.Now,
                            };
                            _context.credit_customercarddetails.Add(loanCustomerCardDetails);
                        }
                    }
                }
                var response = _context.SaveChanges() > 0;
                return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Records Uploaded Successfully" } }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LoanCustomerRespObj UpdateLoanCustomerByCustomer(LoanCustomerObj entity)
        {
            try
            {
                using (var trans = _context.Database.BeginTransaction())
                {
                    credit_loancustomer loanCustomer = null;
                    var accountNumber = GenerateRandomDigitCode(10);
                    loanCustomer = _context.credit_loancustomer.Find(entity.CustomerId);
                    if (loanCustomer != null)
                    {
                        if (loanCustomer.CASAAccountNumber == null)
                        {
                            loanCustomer.CASAAccountNumber = accountNumber;
                        }
                        loanCustomer.CustomerTypeId = entity.CustomerTypeId;
                        loanCustomer.TitleId = entity.TitleId;
                        loanCustomer.FirstName = entity.FirstName;
                        loanCustomer.LastName = entity.LastName;
                        loanCustomer.MiddleName = entity.MiddleName;
                        loanCustomer.GenderId = entity.GenderId;
                        loanCustomer.DOB = entity.Dob;
                        loanCustomer.Address = entity.Address;
                        loanCustomer.PostaAddress = entity.PostalAddress;
                        loanCustomer.Email = entity.Email;
                        loanCustomer.EmploymentType = entity.EmploymentType;
                        loanCustomer.IncorporationCountry = entity.IncorporationCountry;
                        loanCustomer.Industry = entity.Industry;
                        loanCustomer.ShareholderFund = entity.ShareholderFund;
                        loanCustomer.CityId = entity.CityId;
                        loanCustomer.City = entity.City;
                        loanCustomer.Occupation = entity.Occupation;
                        loanCustomer.PoliticallyExposed = entity.PoliticallyExposed;
                        loanCustomer.CompanyName = entity.CompanyName;
                        loanCustomer.CompanyWebsite = entity.CompanyWebsite;
                        loanCustomer.RegistrationNo = entity.RegistrationNo;
                        loanCustomer.CountryId = entity.CountryId;
                        loanCustomer.PhoneNo = entity.PhoneNo;
                        loanCustomer.MaritalStatusId = entity.MaritalStatusId;
                        loanCustomer.AnnualTurnover = entity.AnnualTurnover;
                        loanCustomer.Active = true;
                        loanCustomer.Deleted = false;
                        loanCustomer.UpdatedBy = "Customer";
                        loanCustomer.UpdatedOn = DateTime.Now;
                        loanCustomer.ProfileStatus = entity.ProfileStatus;
                        //UpdateUserAsync(loanCustomer);
                        updateCASA(entity, loanCustomer.CustomerId, loanCustomer.CASAAccountNumber); 
                    }
                    else
                    {
                        loanCustomer = new credit_loancustomer
                        {
                            CustomerTypeId = entity.CustomerTypeId,
                            TitleId = entity.TitleId,
                            FirstName = entity.FirstName,
                            LastName = entity.LastName,
                            MiddleName = entity.MiddleName,
                            GenderId = entity.GenderId,
                            Address = entity.Address,
                            DOB = entity.Dob,
                            RegistrationSource = "Website",
                            PostaAddress = entity.PostalAddress,
                            Email = entity.Email,
                            EmploymentType = entity.EmploymentType,
                            IncorporationCountry = entity.IncorporationCountry,
                            Industry = entity.Industry,
                            ShareholderFund = entity.ShareholderFund,
                            CityId = entity.CityId,
                            Occupation = entity.Occupation,
                            PoliticallyExposed = entity.PoliticallyExposed,
                            CompanyName = entity.CompanyName,
                            CompanyWebsite = entity.CompanyWebsite,
                            RegistrationNo = entity.RegistrationNo,
                            AnnualTurnover = entity.AnnualTurnover,
                            //ApprovalStatusId = entity.approvalStatusId,
                            CountryId = entity.CountryId,
                            PhoneNo = entity.PhoneNo,
                            MaritalStatusId = entity.MaritalStatusId,
                            CASAAccountNumber = entity.CASAAccountNumber,
                            Active = true,
                            Deleted = false,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = DateTime.Now,
                            ProfileStatus = entity.ProfileStatus
                        };
                        _context.credit_loancustomer.Add(loanCustomer);
                        updateCASA(entity, loanCustomer.CustomerId, entity.CASAAccountNumber); 
                    }
                    try
                    {
                        var response = _context.SaveChanges() > 0;

                        if (response)
                        {
                            trans.Commit();
                            return new LoanCustomerRespObj()
                            {
                                CustomerId = loanCustomer.CustomerId,
                                Status = new APIResponseStatus
                                {
                                    IsSuccessful = true,
                                    Message = new APIResponseMessage
                                    {
                                        FriendlyMessage = "Record saved Successfully"
                                    }
                                }
                            };
                        }
                        trans.Rollback();
                        return new LoanCustomerRespObj()
                        {
                            Status = new APIResponseStatus
                            {
                                IsSuccessful = false,
                                Message = new APIResponseMessage
                                {
                                    FriendlyMessage = "Record not saved"
                                }
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateLoanCustomerDirector(LoanCustomerDirectorObj entity)
        {
            try
            {
                if (entity == null) return false;

                if (entity.CustomerDirectorId > 0)
                {
                    var loanCustomerDirectorExist = _context.credit_loancustomerdirector.Find(entity.CustomerDirectorId);
                    if (loanCustomerDirectorExist != null)
                    {
                        loanCustomerDirectorExist.DirectorTypeId = entity.DirectorTypeId;
                        loanCustomerDirectorExist.PercentageShare = entity.PercentageShare;
                        loanCustomerDirectorExist.Name = entity.Name;
                        loanCustomerDirectorExist.Position = entity.Position;
                        loanCustomerDirectorExist.PoliticallyPosition = entity.PoliticallyPosition;
                        loanCustomerDirectorExist.RelativePoliticallyPosition = entity.RelativePoliticallyPosition;
                        loanCustomerDirectorExist.PhoneNo = entity.PhoneNo;
                        loanCustomerDirectorExist.Email = entity.Email;
                        loanCustomerDirectorExist.Address = entity.Address;
                        loanCustomerDirectorExist.Signature = entity.Signature;
                        loanCustomerDirectorExist.DateOfBirth = entity.Dob;
                        loanCustomerDirectorExist.CustomerId = entity.CustomerId;
                    }
                }
                else
                {
                    var loanCustomerDirector = new credit_loancustomerdirector
                    {
                        DirectorTypeId = entity.DirectorTypeId,
                        PercentageShare = entity.PercentageShare,
                        Name = entity.Name,
                        Position = entity.Position,
                        PoliticallyPosition = entity.PoliticallyPosition,
                        RelativePoliticallyPosition = entity.RelativePoliticallyPosition,
                        PhoneNo = entity.PhoneNo,
                        Email = entity.Email,
                        Address = entity.Address,
                        Signature = entity.Signature,
                        DateOfBirth = entity.Dob,
                        CustomerId = entity.CustomerId,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    _context.credit_loancustomerdirector.Add(loanCustomerDirector);
                }
                var response = _context.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LoanCustomerRespObj> UploadDirectors(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                List<LoanCustomerDirectorObj> uploadedRecord = new List<LoanCustomerDirectorObj>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            uploadedRecord.Add(new LoanCustomerDirectorObj
                            {
                                CustomerEmail = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                Name = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                Position = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                                Email = workSheet.Cells[i, 4].Value.ToString() != null ? workSheet.Cells[i, 4].Value.ToString() : "",
                                Dob = workSheet.Cells[i, 5].Value != null ? DateTime.Parse(workSheet.Cells[i, 5].Value.ToString()) : DateTime.Now,
                                PhoneNo = workSheet.Cells[i, 6].Value.ToString() != null ? workSheet.Cells[i, 6].Value.ToString() : "",
                                Address = workSheet.Cells[i, 7].Value.ToString() != null ? workSheet.Cells[i, 7].Value.ToString() : "",
                                PercentageShare = Convert.ToDecimal(workSheet.Cells[i, 8].Value) != 0 ? Convert.ToDecimal(workSheet.Cells[i, 8].Value) : 0,
                                DirectorType = workSheet.Cells[i, 9].Value.ToString() != null ? workSheet.Cells[i, 9].Value.ToString() : "",
                                PoliticallyPosition = Convert.ToBoolean(workSheet.Cells[i, 10].Value.ToString()) != null ? Convert.ToBoolean(workSheet.Cells[i, 10].Value.ToString()) : false,
                            });
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    foreach (var entity in uploadedRecord)
                    {

                        if (entity.CustomerEmail == "" || entity.CustomerEmail == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a corporate customer's email" } }
                        };
                        var accountTypeExist = _context.credit_loancustomer.Where(x => x.Email.ToLower().Trim() == entity.CustomerEmail.ToLower().Trim()).FirstOrDefault();
                        if (accountTypeExist == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Corporate Customer's email does not match any user" } }
                        };
                        if (entity.Email == "" || entity.Email == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a Director's email" } }
                        };
                        if (entity.DirectorType.ToLower().Trim() == "director")
                        {
                            entity.DirectorTypeId = 1;
                        } else if (entity.DirectorType.ToLower().Trim() == "shareholder")
                        {
                            entity.DirectorTypeId = 2;
                        } else
                        {
                            entity.DirectorTypeId = 3;
                        }
                        var loanCustomerDirectorExist = _context.credit_loancustomerdirector.FirstOrDefault(x=>x.Email.ToLower().Trim() == entity.Email.ToLower().Trim());
                        if (loanCustomerDirectorExist != null)
                        {
                            loanCustomerDirectorExist.DirectorTypeId = entity.DirectorTypeId;
                            loanCustomerDirectorExist.PercentageShare = entity.PercentageShare;
                            loanCustomerDirectorExist.Name = entity.Name;
                            loanCustomerDirectorExist.Position = entity.Position;
                            loanCustomerDirectorExist.PoliticallyPosition = entity.PoliticallyPosition;
                            loanCustomerDirectorExist.RelativePoliticallyPosition = entity.RelativePoliticallyPosition;
                            loanCustomerDirectorExist.PhoneNo = entity.PhoneNo;
                            loanCustomerDirectorExist.Email = entity.Email;
                            loanCustomerDirectorExist.Address = entity.Address;
                            loanCustomerDirectorExist.Signature = entity.Signature;
                            loanCustomerDirectorExist.DateOfBirth = entity.Dob;
                            loanCustomerDirectorExist.CustomerId = accountTypeExist.CustomerId;
                        }
                        else
                        {
                            var loanCustomerDirector = new credit_loancustomerdirector
                            {
                                DirectorTypeId = entity.DirectorTypeId,
                                PercentageShare = entity.PercentageShare,
                                Name = entity.Name,
                                Position = entity.Position,
                                PoliticallyPosition = entity.PoliticallyPosition,
                                RelativePoliticallyPosition = entity.RelativePoliticallyPosition,
                                PhoneNo = entity.PhoneNo,
                                Email = entity.Email,
                                Address = entity.Address,
                                Signature = entity.Signature,
                                DateOfBirth = entity.Dob,
                                CustomerId = accountTypeExist.CustomerId,
                                Active = true,
                                Deleted = false,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = DateTime.Now,
                            };
                            _context.credit_loancustomerdirector.Add(loanCustomerDirector);
                        }
                    }
                }
                var response = _context.SaveChanges() > 0;
                return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Records Uploaded Successfully" } }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public bool UpdateLoanCustomerDirectorShareHolder(LoanCustomerDirectorShareHolderObj entity)
        {
            try
            {

                if (entity == null) return false;


                if (entity.DirectorShareHolderId > 0)
                {
                    var loanCustomerDirectorShareHolderExist = _context.credit_directorshareholder.Find(entity.DirectorShareHolderId);
                    if (loanCustomerDirectorShareHolderExist != null)
                    {
                        loanCustomerDirectorShareHolderExist.CompanyName = entity.CompanyName;
                        loanCustomerDirectorShareHolderExist.PercentageHolder = entity.PercentageHolder;
                        loanCustomerDirectorShareHolderExist.CustomerId = entity.CustomerId;
                        loanCustomerDirectorShareHolderExist.Active = true;
                        loanCustomerDirectorShareHolderExist.Deleted = false;
                        loanCustomerDirectorShareHolderExist.UpdatedBy = entity.CreatedBy;
                        loanCustomerDirectorShareHolderExist.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    var loanCustomerDirectorShareHolder = new credit_directorshareholder
                    {
                        CompanyName = entity.CompanyName,
                        PercentageHolder = entity.PercentageHolder,
                        CustomerId = entity.CustomerId,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    _context.credit_directorshareholder.Add(loanCustomerDirectorShareHolder);
                }

                var response = _context.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateLoanCustomerDocument(LoanCustomerDocumentObj entity)
        {
            try
            {
                if (entity == null) return false;

                if (entity.CustomerDocumentId > 0)
                {
                    var loanCustomerDocumentExist = _context.credit_loancustomerdocument.Find(entity.CustomerDocumentId);
                    if (loanCustomerDocumentExist != null)
                    {
                        loanCustomerDocumentExist.DocumentTypeId = entity.DocumentTypeId;
                        loanCustomerDocumentExist.DocumentExtension = entity.DocumentExtension;
                        loanCustomerDocumentExist.DocumentName = entity.DocumentName;
                        loanCustomerDocumentExist.DocumentFile = entity.DocumentFile;
                        loanCustomerDocumentExist.PhysicalLocation = entity.PhysicalLocation;
                        loanCustomerDocumentExist.CustomerId = entity.CustomerId;
                        loanCustomerDocumentExist.Active = true;
                        loanCustomerDocumentExist.Deleted = false;
                        loanCustomerDocumentExist.UpdatedBy = entity.CreatedBy;
                        loanCustomerDocumentExist.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    var loanCustomerDocument = new credit_loancustomerdocument
                    {
                        DocumentTypeId = entity.DocumentTypeId,
                        DocumentExtension = entity.DocumentExtension,
                        DocumentName = entity.DocumentName,
                        DocumentFile = entity.DocumentFile,
                        PhysicalLocation = entity.PhysicalLocation,
                        CustomerId = entity.CustomerId,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                    };
                    _context.credit_loancustomerdocument.Add(loanCustomerDocument);
                }

                var response = _context.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateLoanCustomerFromWebsite(LoanCustomerObj entity)
        {
            try
            {
                //if (entity == null) return false;
                if (entity == null) return false;
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(entity.Password, out passwordHash, out passwordSalt);
                credit_loancustomer loanCustomer = null;
                var accountNumber = GenerateRandomDigitCode(10);
                if (entity.CASAAccountNumber == null)
                {
                    entity.CASAAccountNumber = accountNumber;
                }
                if (entity.CustomerId > 0)
                {
                    loanCustomer = _context.credit_loancustomer.Find(entity.CustomerId);
                    if (loanCustomer != null)
                    {
                        loanCustomer.CustomerTypeId = entity.CustomerTypeId;
                        loanCustomer.TitleId = entity.TitleId;
                        loanCustomer.FirstName = entity.FirstName;
                        loanCustomer.LastName = entity.LastName;
                        loanCustomer.MiddleName = entity.MiddleName;
                        loanCustomer.GenderId = entity.GenderId;
                        loanCustomer.DOB = entity.Dob;
                        loanCustomer.Address = entity.Address;
                        loanCustomer.PostaAddress = entity.PostalAddress;
                        loanCustomer.Email = entity.Email;
                        loanCustomer.EmploymentType = entity.EmploymentType;
                        loanCustomer.IncorporationCountry = entity.IncorporationCountry;
                        loanCustomer.Industry = entity.Industry;
                        loanCustomer.ShareholderFund = entity.ShareholderFund;
                        loanCustomer.CityId = entity.CityId;
                        loanCustomer.Occupation = entity.Occupation;
                        loanCustomer.PoliticallyExposed = entity.PoliticallyExposed;
                        loanCustomer.CompanyName = entity.CompanyName;
                        loanCustomer.CompanyWebsite = entity.CompanyWebsite;
                        loanCustomer.RegistrationNo = entity.RegistrationNo;
                        loanCustomer.CountryId = entity.CountryId;
                        loanCustomer.PhoneNo = entity.PhoneNo;
                        loanCustomer.MaritalStatusId = entity.MaritalStatusId;
                        loanCustomer.RelationshipManagerId = entity.RelationshipOfficerId;
                        loanCustomer.AnnualTurnover = entity.AnnualTurnover;
                        loanCustomer.Active = true;
                        loanCustomer.Deleted = false;
                        loanCustomer.UpdatedBy = entity.CreatedBy;
                        loanCustomer.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    loanCustomer = new credit_loancustomer
                    {
                        CustomerTypeId = entity.CustomerTypeId,
                        TitleId = entity.TitleId,
                        FirstName = entity.FirstName,
                        LastName = entity.LastName,
                        MiddleName = entity.MiddleName,
                        GenderId = entity.GenderId,
                        RegistrationSource = "Website",
                        Signature = entity.Signature,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Address = entity.Address,
                        DOB = entity.Dob,
                        PostaAddress = entity.PostalAddress,
                        Email = entity.Email,
                        EmploymentType = entity.EmploymentType,
                        IncorporationCountry = entity.IncorporationCountry,
                        Industry = entity.Industry,
                        ShareholderFund = entity.ShareholderFund,
                        CityId = entity.CityId,
                        Occupation = entity.Occupation,
                        PoliticallyExposed = entity.PoliticallyExposed,
                        CompanyName = entity.CompanyName,
                        CompanyWebsite = entity.CompanyWebsite,
                        RegistrationNo = entity.RegistrationNo,
                        AnnualTurnover = entity.AnnualTurnover,
                        ApprovalStatusId = entity.ApprovalStatusId,
                        CountryId = entity.CountryId,
                        PhoneNo = entity.PhoneNo,
                        MaritalStatusId = entity.MaritalStatusId,
                        RelationshipManagerId = entity.RelationshipOfficerId,
                        CASAAccountNumber = entity.CASAAccountNumber,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                        ProfileStatus = 0,
                    };
                    _context.credit_loancustomer.Add(loanCustomer);
                }

                using (var trans = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.SaveChanges();

                        bool output = true;
                        //var baseFrontendURL = mail_config.BaseFrontEndURL;
                        //var accountId = loanCustomer.CustomerId;
                        //StringBuilder sbEmailBody = new StringBuilder();
                        //sbEmailBody.Append("<td style =\"font-family: sans-serif; font-size: 14px; vertical-align: top;>\"");
                        //sbEmailBody.Append("<p style =\"font -family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\">");
                        //sbEmailBody.Append($"Hi {loanCustomer.FirstName},");
                        //sbEmailBody.Append($"<br/><br/><br/></p><p style =\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\"> Welcome to GOS Credit! There's just one step before you get to complete your customer account registration. Verify you have the right email address by clicking on the button below.</p>");
                        //sbEmailBody.Append("<table border =\"0\" cellpadding =\"0\" cellspacing =\"0\" class=\"btn btn-primary\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100 %; box-sizing: border-box;\"><tbody><tr>");
                        //sbEmailBody.Append("<td align =\"left\" style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; padding-bottom: 15px;\">");
                        //sbEmailBody.Append("<table border = \"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;\">");
                        //sbEmailBody.Append("<tbody><tr>");
                        //sbEmailBody.Append($"<td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #3498db; border-radius: 5px; text-align: center;\"><a href=\"{baseFrontendURL + "/#/auth/activate-account/" + accountId }\" target=\"_blank\" style=\"display: inline-block; color: #ffffff; background-color: #3498db; border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-transform: capitalize; border-color: #3498db;\">verify my Account</a></td></tr>");

                        //sbEmailBody.Append("</tbody></table></td></tr></tbody>");
                        //sbEmailBody.Append("</table>");
                        //sbEmailBody.Append("<p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\"> Once your account creation is completed, your can explore our services and have a seamless experience.</p>");
                        //sbEmailBody.Append("<br/><br/><br/><p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; Margin-bottom: 15px;\"> Good luck!</p>");
                        //sbEmailBody.Append("</td>");

                        //var mail = new EmailHelpers();
                        //output = mail.SendMail(mail_config, loanCustomer.Email, null, "Email Verification", sbEmailBody.ToString(), "~/EmailTemplate/confirmation.html");


                        //var response = _context.SaveChanges() > 0;

                        if (output)
                        {
                            trans.Commit();
                        }
                        return output;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateLoanCustomerIdentityDetails(LoanCustomerIdentityDetailsObj entity)
        {
            try
            {
                if (entity == null) return false;

                if (entity.CustomerIdentityDetailsId > 0)
                {
                    var loanCustomerIdentityDetailsExist = _context.credit_customeridentitydetails.Find(entity.CustomerIdentityDetailsId);
                    if (loanCustomerIdentityDetailsExist != null)
                    {
                        loanCustomerIdentityDetailsExist.Number = entity.Number;
                        loanCustomerIdentityDetailsExist.Issuer = entity.Issuer;
                        loanCustomerIdentityDetailsExist.IdentificationId = entity.IdentificationId;
                        loanCustomerIdentityDetailsExist.CustomerId = entity.CustomerId;
                        loanCustomerIdentityDetailsExist.Active = true;
                        loanCustomerIdentityDetailsExist.Deleted = false;
                        loanCustomerIdentityDetailsExist.UpdatedBy = entity.CreatedBy;
                        loanCustomerIdentityDetailsExist.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    var loanCustomerIdentityDetails = new credit_customeridentitydetails
                    {
                        Number = entity.Number,
                        Issuer = entity.Issuer,
                        IdentificationId = entity.IdentificationId,
                        CustomerId = entity.CustomerId,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = entity.CreatedBy,
                        UpdatedOn = DateTime.Now,
                    };
                    _context.credit_customeridentitydetails.Add(loanCustomerIdentityDetails);
                }

                var response = _context.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoanCustomerRespObj> UploadCustomerIdentityDetails(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                List<LoanCustomerIdentityDetailsObj> uploadedRecord = new List<LoanCustomerIdentityDetailsObj>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            uploadedRecord.Add(new LoanCustomerIdentityDetailsObj
                            {
                                CustomerEmail = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                Issuer = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                Number = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                            });
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    foreach (var entity in uploadedRecord)
                    {
                        if (entity.CustomerEmail == "" || entity.CustomerEmail == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a valid customer's email" } }
                        };
                        var accountTypeExist = _context.credit_loancustomer.Where(x => x.Email.ToLower().Trim() == entity.CustomerEmail.ToLower().Trim()).FirstOrDefault();
                        if (accountTypeExist == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Customer's email does not match any user" } }
                        };

                        var loanCustomerIdentityDetails = new credit_customeridentitydetails
                        {
                            Number = entity.Number,
                            Issuer = entity.Issuer,
                            IdentificationId = entity.IdentificationId,
                            CustomerId = accountTypeExist.CustomerId,
                            Active = true,
                            Deleted = false,
                            CreatedBy = entity.CreatedBy,
                            CreatedOn = DateTime.Now,
                        };
                        _context.credit_customeridentitydetails.Add(loanCustomerIdentityDetails);
                    }
                }
                var response = _context.SaveChanges() > 0;
                return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Records Uploaded Successfully" } }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateLoanCustomerNextOfKin(LoanCustomerNextOfKinObj entity)
        {
            try
            {
                if (entity == null) return false;

                if (entity.CustomerNextOfKinId > 0)
                {
                    var loanCustomerNextOfKinExist = _context.credit_customernextofkin.Find(entity.CustomerNextOfKinId);
                    if (loanCustomerNextOfKinExist != null)
                    {
                        loanCustomerNextOfKinExist.Name = entity.Name;
                        loanCustomerNextOfKinExist.Relationship = entity.Relationship;
                        loanCustomerNextOfKinExist.PhoneNo = entity.PhoneNo;
                        loanCustomerNextOfKinExist.Email = entity.Email;
                        loanCustomerNextOfKinExist.Address = entity.Address;
                        loanCustomerNextOfKinExist.CustomerId = entity.CustomerId;
                        loanCustomerNextOfKinExist.Active = true;
                        loanCustomerNextOfKinExist.Deleted = false;
                        loanCustomerNextOfKinExist.CreatedBy = entity.CreatedBy;
                        loanCustomerNextOfKinExist.CreatedOn = DateTime.Now;
                        loanCustomerNextOfKinExist.UpdatedBy = entity.CreatedBy;
                        loanCustomerNextOfKinExist.UpdatedOn = DateTime.Now;
                    }
                }
                else
                {
                    var loanCustomerNextOfKin = new credit_customernextofkin
                    {
                        Name = entity.Name,
                        Relationship = entity.Relationship,
                        PhoneNo = entity.PhoneNo,
                        Email = entity.Email,
                        Address = entity.Address,
                        CustomerId = entity.CustomerId,
                        Active = true,
                        Deleted = false,
                        CreatedBy = entity.CreatedBy,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = entity.CreatedBy,
                        UpdatedOn = DateTime.Now,
                    };
                    _context.credit_customernextofkin.Add(loanCustomerNextOfKin);
                }

                var response = _context.SaveChanges() > 0;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoanCustomerRespObj> UploadCustomerNextOfKin(List<byte[]> record, string createdBy)
        {
            try
            {
                if (record == null) return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                List<LoanCustomerNextOfKinObj> uploadedRecord = new List<LoanCustomerNextOfKinObj>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            uploadedRecord.Add(new LoanCustomerNextOfKinObj
                            {
                                CustomerEmail = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                Name = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                Relationship = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                                Email = workSheet.Cells[i, 4].Value.ToString() != null ? workSheet.Cells[i, 4].Value.ToString() : "",
                                PhoneNo = workSheet.Cells[i, 5].Value.ToString() != null ? workSheet.Cells[i, 5].Value.ToString() : "",
                                Address = workSheet.Cells[i, 6].Value.ToString() != null ? workSheet.Cells[i, 6].Value.ToString() : "",
                            });
                        }
                    }
                }

                if (uploadedRecord.Count > 0)
                {
                    foreach (var entity in uploadedRecord)
                    {
                        if (entity.CustomerEmail == "" || entity.CustomerEmail == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a corporate customer's email" } }
                        };
                        var accountTypeExist = _context.credit_loancustomer.Where(x => x.Email.ToLower().Trim() == entity.CustomerEmail.ToLower().Trim()).FirstOrDefault();
                        if (accountTypeExist == null) return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Customer's email does not match any user" } }
                        };
                        if (entity.Email == "") return new LoanCustomerRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Please include a Next of KIN's email" } }
                        };
                        var loanCustomerNextOfKinExist = _context.credit_customernextofkin.FirstOrDefault(x => x.Email.ToLower().Trim() == entity.Email.ToLower().Trim());
                        if (loanCustomerNextOfKinExist != null)
                        {
                            loanCustomerNextOfKinExist.Name = entity.Name;
                            loanCustomerNextOfKinExist.Relationship = entity.Relationship;
                            loanCustomerNextOfKinExist.PhoneNo = entity.PhoneNo;
                            loanCustomerNextOfKinExist.Email = entity.Email;
                            loanCustomerNextOfKinExist.Address = entity.Address;
                            loanCustomerNextOfKinExist.CustomerId = accountTypeExist.CustomerId;
                        }
                        else
                        {
                            var loanCustomerNextOfKin = new credit_customernextofkin
                            {
                                Name = entity.Name,
                                Relationship = entity.Relationship,
                                PhoneNo = entity.PhoneNo,
                                Email = entity.Email,
                                Address = entity.Address,
                                CustomerId = accountTypeExist.CustomerId,
                                Active = true,
                                Deleted = false,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = DateTime.Now,
                                UpdatedBy = entity.CreatedBy,
                                UpdatedOn = DateTime.Now,
                            };
                            _context.credit_customernextofkin.Add(loanCustomerNextOfKin);
                        }
                    }
                }
                var response = _context.SaveChanges() > 0;
                return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Records Uploaded Successfully" } }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UploadDocumentType(byte[] record, string createdBy)
        {
            try
            {
                if (record == null) return false;
                List<DocumentTypeObj> uploadedRecord = new List<DocumentTypeObj>();
                using (MemoryStream stream = new MemoryStream(record))
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    //Use first sheet by default
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];
                    int totalRows = workSheet.Dimension.Rows;
                    //First row is considered as the header
                    for (int i = 2; i <= totalRows; i++)
                    {
                        uploadedRecord.Add(new DocumentTypeObj
                        {
                            DocumentTypeName = workSheet.Cells[i, 1].Value.ToString(),

                        });
                    }
                }
                if (uploadedRecord.Count > 0)
                {
                    foreach (var entity in uploadedRecord)
                    {
                        //var documenttype = _context.credit_documenttype.Where(x => x.Name == entity.documentTypeName && x.Deleted == false).FirstOrDefault();
                        //if (documenttype == null)
                        //{
                        //    throw new Exception("Please include a valid Document Name");
                        //}
                        if (entity.DocumentTypeName == "")
                        {
                            throw new Exception("Please include a valid Document Name");
                        }

                        var documentExist = _context.credit_documenttype.Where(x => x.Name == entity.DocumentTypeName && x.Deleted == false).FirstOrDefault();
                        if (documentExist != null)
                        {
                            documentExist.Name = entity.DocumentTypeName;
                            documentExist.Active = true;
                            documentExist.Deleted = false;
                            documentExist.UpdatedBy = entity.CreatedBy;
                            documentExist.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            var accountType = new credit_documenttype
                            {
                                Name = entity.DocumentTypeName,
                                DocumentTypeId = entity.DocumentTypeId,
                                Active = true,
                                Deleted = false,
                                UpdatedBy = entity.CreatedBy,
                                UpdatedOn = DateTime.Now,
                            };
                            _context.credit_documenttype.Add(accountType);
                        }
                    }
                }
                var response = _context.SaveChanges() > 0;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoanCustomerRespObj> UploadIndividualCustomer(List<byte[]> record, string createdBy)
        {
            var response = new LoanCustomerRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
            try
            {
             
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                List<LoanCustomerObj> uploadedRecord = new List<LoanCustomerObj>();
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {
                            uploadedRecord.Add(new LoanCustomerObj
                            {
                                Excel_line_number = i,
                                Email = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                CustomerTypeName = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                Title = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                                FirstName = workSheet.Cells[i, 4].Value.ToString() != null ? workSheet.Cells[i, 4].Value.ToString() : "",
                                MiddleName = workSheet.Cells[i, 5].Value.ToString() != null ? workSheet.Cells[i, 5].Value.ToString() : "",
                                LastName = workSheet.Cells[i, 6].Value.ToString() != null ? workSheet.Cells[i, 6].Value.ToString() : "",
                                Gender = workSheet.Cells[i, 7].Value.ToString() != null ? workSheet.Cells[i, 7].Value.ToString() : "",
                                Address = workSheet.Cells[i, 8].Value.ToString() != null ? workSheet.Cells[i, 8].Value.ToString() : "",
                                Dob = Convert.ToDateTime(workSheet.Cells[i, 9].Value) != null ? Convert.ToDateTime(workSheet.Cells[i, 9].Value.ToString()) : DateTime.Now,
                                EmploymentTypeName = workSheet.Cells[i, 10].Value.ToString() != null ? workSheet.Cells[i, 10].Value.ToString() : "",
                                City = workSheet.Cells[i, 11].Value.ToString() != null ? workSheet.Cells[i, 11].Value.ToString() : "",
                                Occupation = workSheet.Cells[i, 12].Value.ToString() != null ? workSheet.Cells[i, 12].Value.ToString() : "",
                                PoliticallyExposed = Convert.ToBoolean(workSheet.Cells[i, 13].Value) != null ? bool.Parse(workSheet.Cells[i, 13].Value.ToString()) : false,
                                Country = workSheet.Cells[i, 14].Value.ToString() != null ? workSheet.Cells[i, 14].Value.ToString() : "",
                                PhoneNo = workSheet.Cells[i, 15].Value.ToString() != null ? workSheet.Cells[i, 15].Value.ToString() : "",
                                MaritalStatus = workSheet.Cells[i, 16].Value.ToString() != null ? workSheet.Cells[i, 16].Value.ToString() : "",
                            });
                        }
                    }
                }
                var titleList = await _identityServer.GetTitleAsync();
                var employTypeList = await _identityServer.GetEmploymentTypeAsync();
                var getMaritalStatusList = await _identityServer.GetMaritalStatusAsync();
                //var getCityList = await _identityServer.GetCitiesAsync();
                var getCountryList = await _identityServer.GetCountriesAsync();
                var getGenderList = await _identityServer.GetGenderAsync();

                if (uploadedRecord.Count > 0)
                {
                    try
                    {
                        foreach (var entity in uploadedRecord)
                        {
                            if (entity.CustomerTypeName == "" || entity.PhoneNo == "" || entity.Address == "" || entity.FirstName == "" || entity.Email == "")
                            {
                                response.Status.Message.FriendlyMessage = $"Please include relative fields(Email, Phone, Address, FirstName, CustomerTypeName) detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            var customerTypeId = entity.CustomerTypeName.ToLower().Trim() == "individual" ? 1 : 2;

                            var title = titleList.commonLookups.Where(x => x.LookupName.ToLower().Trim() == entity.Title.ToLower().Trim()).FirstOrDefault();
                            if (title == null)
                            {
                                response.Status.Message.FriendlyMessage = $"Please include a valid Title detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            var gender = getGenderList.commonLookups.Where(x => x.LookupName.ToLower().Trim() == entity.Gender.ToLower().Trim()).FirstOrDefault();
                            if (gender == null)
                            {
                                response.Status.Message.FriendlyMessage = "Please include a valid Gender";
                                return response;
                            }
                            var marital = getMaritalStatusList.commonLookups.Where(x => x.LookupName.ToLower().Trim() == entity.MaritalStatus.ToLower().Trim()).FirstOrDefault();
                            if (marital == null)
                            {
                                response.Status.Message.FriendlyMessage = "Please include a valid Marital Status detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            var country = getCountryList.commonLookups.Where(x => x.LookupName.ToLower().Trim() == entity.Country.ToLower().Trim()).FirstOrDefault();
                            if (country == null)
                            {
                                response.Status.Message.FriendlyMessage = "Please include a valid Country detected on line {entity.Excel_line_number}";
                                return response;
                            }

                            var employmentType = employTypeList.commonLookups.Where(x => x.LookupName.ToLower().Trim() == entity.EmploymentTypeName.ToLower().Trim()).FirstOrDefault();
                            if (employmentType == null)
                            {
                                response.Status.Message.FriendlyMessage = "Please include a valid Employment Type detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            entity.ApprovalStatusId = 0;
                            var accountNumber = GenerateRandomDigitCode(10);
                            var existing_customer = _context.credit_loancustomer.Where(x => x.Email.ToLower().Trim() == entity.Email.ToLower().Trim()).FirstOrDefault();

                            if (existing_customer != null)
                            {
                                existing_customer.CustomerTypeId = customerTypeId;
                                existing_customer.TitleId = title.LookupId;
                                existing_customer.FirstName = entity.FirstName;
                                existing_customer.LastName = entity.LastName;
                                existing_customer.MiddleName = entity.MiddleName;
                                existing_customer.GenderId = gender.LookupId;
                                existing_customer.Address = entity.Address;
                                existing_customer.DOB = entity.Dob;
                                existing_customer.PostaAddress = entity.PostalAddress;
                                existing_customer.Email = entity.Email;
                                existing_customer.EmploymentType = entity.EmploymentType;
                                existing_customer.IncorporationCountry = entity.IncorporationCountry;
                                existing_customer.Industry = entity.Industry;
                                existing_customer.ShareholderFund = entity.ShareholderFund;
                                existing_customer.City = entity.City;
                                existing_customer.Occupation = entity.Employment;
                                existing_customer.PoliticallyExposed = entity.PoliticallyExposed;
                                existing_customer.CompanyName = entity.CompanyName;
                                existing_customer.CompanyWebsite = entity.CompanyWebsite;
                                existing_customer.RegistrationNo = entity.RegistrationNo;
                                existing_customer.AnnualTurnover = entity.AnnualTurnover;
                                existing_customer.CountryId = country.LookupId;
                                existing_customer.PhoneNo = entity.PhoneNo;
                                existing_customer.MaritalStatusId = marital.LookupId;
                                existing_customer.ApprovalStatusId = entity.ApprovalStatusId;
                                existing_customer.RelationshipManagerId = entity.RelationshipOfficerId;
                                await _context.SaveChangesAsync();

                                var identity_result = await AddUpdateAsUserAsync(existing_customer);

                                if (!identity_result.Succeeded)
                                { 
                                    response.Status.Message.FriendlyMessage = identity_result.Errors.FirstOrDefault().Description;
                                    if (identity_result.Errors.Any(s => s.Code == $"DuplicateUserName"))
                                    {
                                        response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                    }
                                    if (identity_result.Errors.Any(s => s.Code == $"InvalidUserName"))
                                    {
                                        response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                    }
                                    return response;
                                }
                                var user = _userManager.Users.Where(s => s.Email.Trim().ToLower() == existing_customer.Email.Trim().ToLower()).ToList();
                                if (user.Count() > 1)
                                { 
                                    response.Status.Message.FriendlyMessage = $"Duplicate Email  Detected detected on line {entity.Excel_line_number}";
                                    return response;
                                }
                            }
                            else
                            {
                                var new_customer = new credit_loancustomer
                                {
                                    CustomerId = entity.CustomerId,
                                    CustomerTypeId = customerTypeId,
                                    CASAAccountNumber = accountNumber,
                                    TitleId = title.LookupId,
                                    FirstName = entity.FirstName,
                                    LastName = entity.LastName,
                                    MiddleName = entity.MiddleName,
                                    GenderId = gender.LookupId,
                                    Address = entity.Address,
                                    DOB = entity.Dob,
                                    PostaAddress = entity.PostalAddress,
                                    Email = entity.Email,
                                    EmploymentType = employmentType.LookupId,
                                    IncorporationCountry = entity.IncorporationCountry,
                                    Industry = entity.Industry,
                                    ShareholderFund = entity.ShareholderFund,
                                    City = entity.City,
                                    Occupation = entity.Occupation,
                                    PoliticallyExposed = entity.PoliticallyExposed,
                                    CompanyName = entity.CompanyName,
                                    CompanyWebsite = entity.CompanyWebsite,
                                    RegistrationNo = entity.RegistrationNo,
                                    AnnualTurnover = entity.AnnualTurnover,
                                    CountryId = country.LookupId,
                                    PhoneNo = entity.PhoneNo,
                                    MaritalStatusId = marital.LookupId,
                                    ApprovalStatusId = entity.ApprovalStatusId,
                                    RelationshipManagerId = entity.RelationshipOfficerId,
                                    Active = true,
                                    Deleted = false,
                                    CreatedBy = entity.CreatedBy,
                                    CreatedOn = DateTime.Now,
                                };
                                _context.credit_loancustomer.Add(new_customer);
                                await _context.SaveChangesAsync();

                                updateCASA(entity, new_customer.CustomerId, accountNumber);

                                var identity_result = await AddUpdateAsUserAsync(new_customer);
                                if (!identity_result.Succeeded)
                                { 
                                    response.Status.Message.FriendlyMessage = identity_result.Errors.FirstOrDefault().Description;
                                    if (identity_result.Errors.Any(s => s.Code == "DuplicateUserName"))
                                    {
                                        response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                    }
                                    if (identity_result.Errors.Any(s => s.Code == "InvalidUserName"))
                                    {
                                        response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                    }
                                    return response;
                                }
                            }
                        }

                        await _context.SaveChangesAsync(); 
                    }
                    catch (Exception ex)
                    { 
                        throw ex;
                    } 
                }
                return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Records Uploaded Successfully" } }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoanCustomerRespObj> UploadCorporateCustomer(List<byte[]> record, string createdBy)
        {
            var response = new LoanCustomerRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
            try
            {
                if (record == null) return new LoanCustomerRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                List<LoanCustomerObj> uploadedRecord = new List<LoanCustomerObj>();
              
                foreach (var byteItem in record)
                {
                    using (MemoryStream stream = new MemoryStream(byteItem))
                    using (ExcelPackage excelPackage = new ExcelPackage(stream))
                    {
                        //Use first sheet by default
                        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                        int totalRows = workSheet.Dimension.Rows;
                        //First row is considered as the header
                        for (int i = 2; i <= totalRows; i++)
                        {

                            uploadedRecord.Add(new LoanCustomerObj
                            {
                                Excel_line_number = i,
                                Email = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                CustomerTypeName = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                RegistrationNo = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                                CompanyName = workSheet.Cells[i, 4].Value.ToString() != null ? workSheet.Cells[i, 4].Value.ToString() : "",
                                Dob = workSheet.Cells[i, 5].Value != null ? DateTime.Parse(workSheet.Cells[i, 5].Value.ToString()) : DateTime.Now,
                                PhoneNo = workSheet.Cells[i, 6].Value.ToString() != null ? workSheet.Cells[i, 6].Value.ToString() : "",
                                Address = workSheet.Cells[i, 7].Value.ToString() != null ? workSheet.Cells[i, 7].Value.ToString() : "",
                                PostalAddress = workSheet.Cells[i, 8].Value.ToString() != null ? workSheet.Cells[i, 8].Value.ToString() : "",
                                Industry = workSheet.Cells[i, 9].Value.ToString() != null ? workSheet.Cells[i, 9].Value.ToString() : "",
                                IncorporationCountry = workSheet.Cells[i, 10].Value.ToString() != null ? workSheet.Cells[i, 10].Value.ToString() : "",
                                City = workSheet.Cells[i, 11].Value.ToString() != null ? workSheet.Cells[i, 11].Value.ToString() : "",
                                AnnualTurnover = Convert.ToDecimal(workSheet.Cells[i, 12].Value) != 0 ? Convert.ToDecimal(workSheet.Cells[i, 12].Value.ToString()) : 0,
                                ShareholderFund = Convert.ToDecimal(workSheet.Cells[i, 13].Value) != 0 ? Convert.ToDecimal(workSheet.Cells[i, 13].Value.ToString()) : 0,
                                CompanyWebsite = workSheet.Cells[i, 14].Value.ToString() != null ? workSheet.Cells[i, 14].Value.ToString() : ""
                            });
                        }
                    }
                }

                var getCountryList = await _identityServer.GetCountriesAsync();
                var getCompanyList = await _identityServer.GetAllCompanyAsync();

                    try
                    {
                        if (uploadedRecord.Count > 0)
                        { 
                            foreach (var entity in uploadedRecord)
                            {
                                if (entity.CustomerTypeName == "" || entity.PhoneNo == "" || entity.Address == "" || entity.Email == "")
                                {
                                    response.Status.Message.FriendlyMessage = $"Please include relative fields(Email,Phone,Address, CompanyName, CustomerTypeName) detected on line {entity.Excel_line_number}";
                                    return response;
                                }
                                var customerTypeId = entity.CustomerTypeName.ToLower() == "individual" ? 1 : 2;

                                var country = getCountryList.commonLookups.FirstOrDefault(x => x.LookupName.ToLower().Trim() == entity.IncorporationCountry.ToLower().Trim());
                                if (country == null)
                                {
                                    response.Status.Message.FriendlyMessage = $"Please include a valid Country detected on line {entity.Excel_line_number}";
                                    return response;
                                }
                                entity.ApprovalStatusId = 0;
                                var accountNumber = GenerateRandomDigitCode(10);
                                var existing_customer = _context.credit_loancustomer.FirstOrDefault(x => x.Email == entity.Email);

                                if (existing_customer != null)
                                {
                                    existing_customer.CustomerTypeId = customerTypeId;
                                    existing_customer.Address = entity.Address;
                                    existing_customer.DOB = entity.Dob;
                                    existing_customer.PostaAddress = entity.PostalAddress;
                                    existing_customer.Email = entity.Email;
                                    existing_customer.EmploymentType = entity.EmploymentType;
                                    existing_customer.CountryId = country.LookupId;
                                    existing_customer.Industry = entity.Industry;
                                    existing_customer.ShareholderFund = entity.ShareholderFund;
                                    existing_customer.PoliticallyExposed = entity.PoliticallyExposed;
                                    existing_customer.CompanyName = entity.CompanyName;
                                    existing_customer.CompanyWebsite = entity.CompanyWebsite;
                                    existing_customer.RegistrationNo = entity.RegistrationNo;
                                    existing_customer.AnnualTurnover = entity.AnnualTurnover;
                                    existing_customer.PhoneNo = entity.PhoneNo;
                                    existing_customer.ApprovalStatusId = entity.ApprovalStatusId;
                                    existing_customer.FirstName = entity.CompanyName;
                                    existing_customer.City = entity.City;
                                    await _context.SaveChangesAsync();
                                    var identity_result = await AddUpdateAsUserAsync(existing_customer);
                                     
                                    if (!identity_result.Succeeded)
                                    { 
                                        response.Status.Message.FriendlyMessage = identity_result.Errors.FirstOrDefault().Description;
                                        if (identity_result.Errors.Any(s => s.Code == $"DuplicateUserName"))
                                        {
                                            response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                        } 
                                        if (identity_result.Errors.Any(s => s.Code == $"InvalidUserName"))
                                        {
                                            response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                        }
                                        return response;
                                    }
                                    var user = _userManager.Users.Where(s => s.Email.Trim().ToLower() == existing_customer.Email.Trim().ToLower()).ToList();
                                    if (user.Count() > 1)
                                    { 
                                        response.Status.Message.FriendlyMessage = $"Duplicate Email  Detected detected on line {entity.Excel_line_number}";
                                        return response;
                                    }

                                }
                                else
                                {
                                    var new_customer = new credit_loancustomer
                                    {
                                        CustomerTypeId = customerTypeId,
                                        FirstName = entity.CompanyName,
                                        CASAAccountNumber = accountNumber,
                                        Address = entity.Address,
                                        DOB = entity.Dob,
                                        PostaAddress = entity.PostalAddress,
                                        Email = entity.Email,
                                        EmploymentType = entity.EmploymentType,
                                        CountryId = country.LookupId,
                                        Industry = entity.Industry,
                                        ShareholderFund = entity.ShareholderFund,
                                        City = entity.City,
                                        PoliticallyExposed = entity.PoliticallyExposed,
                                        CompanyName = entity.CompanyName,
                                        CompanyWebsite = entity.CompanyWebsite,
                                        RegistrationNo = entity.RegistrationNo,
                                        AnnualTurnover = entity.AnnualTurnover,
                                        PhoneNo = entity.PhoneNo,
                                        MaritalStatusId = entity.MaritalStatusId,
                                        ApprovalStatusId = entity.ApprovalStatusId,
                                        RelationshipManagerId = entity.RelationshipOfficerId,
                                        Active = true,
                                        Deleted = false,
                                        CreatedBy = entity.CreatedBy,
                                        CreatedOn = DateTime.Now,
                                    };
                                    _context.credit_loancustomer.Add(new_customer);
                                    await _context.SaveChangesAsync();

                                    updateCASA(entity, new_customer.CustomerId, accountNumber);

                                    var identity_result = await AddUpdateAsUserAsync(new_customer);

                                    if (!identity_result.Succeeded)
                                    { 
                                        response.Status.Message.FriendlyMessage = identity_result.Errors.FirstOrDefault().Description;
                                        if (identity_result.Errors.Any(s => s.Code == "DuplicateUserName"))
                                        {
                                            response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                        }
                                        if (identity_result.Errors.Any(s => s.Code == "InvalidUserName"))
                                        {
                                            response.Status.Message.FriendlyMessage = $"{identity_result.Errors.FirstOrDefault().Description} detected on line {entity.Excel_line_number}";
                                        }
                                        return response;
                                    } 
                                   // accountType.UserIdentity = identity_result.UserId;
                                }
                            }
                        }
                        await _context.SaveChangesAsync();
                 
                        response.Status.Message.FriendlyMessage = $"Successful";
                        response.Status.IsSuccessful = true;
                        return response;
                    }
                    catch (Exception ex)
                    { 
                        throw ex;
                    }
                   
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        private async Task<IdentityResult> AddUpdateAsUserAsync(credit_loancustomer item)
        {
            IdentityResult result = new IdentityResult();
            var customer = _userManager.Users.FirstOrDefault(x => x.CustomerId == Convert.ToInt64( item == null ? 0 : item.CustomerId));

            if (customer == null)
            {
                var user = new ApplicationUser();
                user.CustomerId = Convert.ToInt64(item.CustomerId);
                user.Address = item?.Address ?? string.Empty;
                user.UserName = item?.Email ?? string.Empty;
                user.Email = item?.Email ?? string.Empty;
                user.PhoneNumber = item?.PhoneNo ?? string.Empty;
                user.FirstName = item?.FirstName ?? string.Empty;
                user.LastName = item?.LastName ?? string.Empty;
                user.CustomerTypeId = item?.CustomerTypeId??0;
                return result = await _userManager.CreateAsync(user, "Password@1");
            }
            //var exUser = await _userManager.FindByIdAsync(staff.Id);
            if (customer != null)
            { 
                customer.CustomerId = Convert.ToInt64(item?.CustomerId??0); 
                customer.Address = item?.Address??string.Empty;
                customer.UserName = item?.Email?? string.Empty;
                customer.Email = item?.Email??string.Empty;
                customer.PhoneNumber = item?.PhoneNo??string.Empty;
                customer.FirstName = item?.FirstName??string.Empty;
                customer.LastName = item?.LastName??string.Empty;
                customer.CustomerTypeId = item?.CustomerTypeId??0; 
                result = await _userManager.UpdateAsync(customer);
            }
            return result;
        }


        public bool verifyEmailAccount(int customerId)
        {
            var customer = _context.credit_loancustomer.Where(x => x.CustomerId == customerId).FirstOrDefault();
            customer.ApprovalStatusId = 2;
            return _context.SaveChanges() > 0;
        }

         
        ///////////////////////PRIVATE METHODSS
        ///
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }
        private static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }

        private void updateCASA(LoanCustomerObj entity, int customerId, string accountNumber)
        {
            var setup = _context.deposit_accountsetup.FirstOrDefault(x => x.DepositAccountId == 3);
            decimal bal = setup.InitialDeposit;
            decimal Ledgerbal = setup.InitialDeposit;
            decimal? irate = setup.InterestRate;
            decimal lienBal = setup.InitialDeposit;

            credit_casa casaAccount = null;
            casaAccount = _context.credit_casa.FirstOrDefault(x => x.AccountNumber == accountNumber);
            if (casaAccount == null)
            {
                var customerAccount = new credit_casa
                {
                    AccountName = entity.FirstName + " " + entity.LastName,
                    AccountNumber = accountNumber,
                    AccountStatusId = (int)CASAAccountStatusEnum.Inactive,
                    ActionDate = DateTime.Now,
                    AprovalStatusId = (int)ApprovalStatus.Pending,
                    AvailableBalance = bal,
                    BranchId = 1,
                    CompanyId = 7,
                    CurrencyId = 1,
                    CustomerId = customerId,
                    //CustomerSensitivityLevelId = entity.customerSensitivityLevelId,
                    EffectiveDate = DateTime.Now,
                    HasLien = true,
                    HasOverdraft = true,
                    InterestRate = irate,
                    IsCurrentAccount = true,
                    LedgerBalance = Ledgerbal,
                    LienAmount = lienBal,
                    //MISCode = "",
                    OperationId = (int)OperationsEnum.CasaAccountApproval,
                    //OverdraftAmount = 0,
                    //OverdraftExpiryDate = entity.overdraftExpiryDate,
                    //OverdraftInterestRate = 0,
                    //PostNoStatusId = entity.postNoStatusId,
                    ProductId = 1,
                    RelationshipManagerId = 0,
                    RelationshipOfficerId = 0,
                    //TEAMMISCode = "",
                    FromDeposit = false,
                    //Tenor = entity.tenor,
                    //TerminalDate = entity.terminalDate,
                    Active = true,
                    Deleted = false,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = DateTime.Now,
                };
                _context.credit_casa.Add(customerAccount);
            }
        }

        //private void updateDepositcustomer(LoanCustomerObj entity, string accountNumber)
        //{
        //    deposit_accountopening casaAccount = new deposit_accountopening();
        //    casaAccount = _context.deposit_accountopening.FirstOrDefault(x => x.AccountNumber == accountNumber)??null;
        //    if (casaAccount == null)
        //    {
        //        var CustomerObj = new deposit_accountopening
        //        {
        //            CustomerTypeId = entity.CustomerTypeId,
        //            AccountTypeId = 3,
        //            AccountNumber = accountNumber,
        //            Title = entity.TitleId,
        //            Surname = entity.LastName,
        //            Firstname = entity.FirstName,
        //            Othername = entity.MiddleName,
        //            MaritalStatusId = entity.MaritalStatusId,
        //            RelationshipOfficerId = entity.RelationshipOfficerId,
        //            GenderId = entity.GenderId,
        //            BirthCountryId = entity.CountryId,
        //            DOB = entity.Dob,
        //            Address1 = entity.Address,
        //            City = entity.PostalAddress,
        //            CountryId = entity.CountryId,
        //            Email = entity.Email,
        //            MobileNumber = entity.PhoneNo,
        //            Active = true,
        //            Deleted = false,
        //            CreatedBy = entity.CreatedBy,
        //            CreatedOn = DateTime.Now,
        //        };
        //        _context.deposit_accountopening.Add(CustomerObj);
        //    }

        //}

        public async Task<IEnumerable<credit_loancustomerdocument>> GetCustomerDocumentsAsync(int customerId)
        {
            return await _context.credit_loancustomerdocument.Where(d => d.CustomerId == customerId && d.Deleted == false).ToListAsync();
        }
    }
}
