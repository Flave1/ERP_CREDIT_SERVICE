using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Banking.AuthHandler.Interface;
using Banking.Contracts.Response.Approvals;
using Banking.Contracts.Response.Mail;
using Banking.Contracts.V1;
using Banking.Data;
using Banking.DomainObjects.Credit;
using Banking.Handlers.Auths;
using Banking.Helpers;
using Banking.Repository.Interface.Credit;
using Banking.Requests;
using GOSLibraries;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using static Banking.Contracts.Response.Credit.ApprovalObjs;
using static Banking.Contracts.Response.Credit.CollateralCustomerObjs;
using static Banking.Contracts.Response.Credit.LoanApplicationObjs;
using static Banking.Contracts.Response.Credit.LoanObjs;

namespace Banking.Controllers.V1.Credit
{
    [ERPAuthorize]
    public class LoanController : Controller
    {
        public List<credit_loan> loans = new List<credit_loan>();
        private readonly ILoanRepository _repo;
        private readonly IIdentityService _identityService;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly ILoanScheduleRepository _schedule;
        private readonly IMapper _mapper;
        ICreditAppraisalRepository _customerTrans;
        IIFRSRepository _ifrs;
        private readonly IFlutterWaveRequest _flutter;
        public LoanController(ILoanRepository repo, IIdentityService identityService, ILoggerService logger, IHttpContextAccessor httpContextAccessor, DataContext context, ICreditAppraisalRepository customerTrans,
        IIdentityServerRequest serverRequest, ILoanScheduleRepository schedule, IMapper mapper, IIFRSRepository ifrs, IFlutterWaveRequest flutter)
        {
            _repo = repo;
            _identityService = identityService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _dataContext = context;
            _serverRequest = serverRequest;
            _schedule = schedule;
            _mapper = mapper;
            _customerTrans = customerTrans;
            _ifrs = ifrs;
            _flutter = flutter;
        }

        #region LOAN
        [HttpPost(ApiRoutes.Loan.ADD_LOAN_BOOKING)]
        public async Task<ActionResult<LoanRespObj>> AddLoanBooking([FromBody] LoanObj model)
        {
            try
            {
                var identity = await _serverRequest.UserDataAsync();
                var user = identity.UserName;

                model.CreatedBy = user;
                model.UpdatedBy = user;
                model.CompanyId = identity.CompanyId;

                var response = await _repo.AddLoanBooking(model);
                return response;

            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = ex?.Message, TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }


        [HttpPost(ApiRoutes.Loan.UPLOAD_LOANS)]
        public async Task<ActionResult<LoanRespObj>> UploadLoans()
        {
            var response = new LoanRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
            try
            {
                var files = _httpContextAccessor.HttpContext.Request.Form.Files;
                var byteList = new List<byte[]>();
                foreach (var fileBit in files)
                {
                    if (fileBit.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileBit.CopyToAsync(ms);
                            byteList.Add(ms.ToArray());
                        }
                    }
                }

                var user = await _serverRequest.UserDataAsync();
                var staffEmail = user.Email;

                if (byteList == null) return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Invalid Payload" } }
                };
                List<LoanObj> uploadedRecord = new List<LoanObj>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //List<credit_loan> loans = new List<credit_loan>();

                foreach (var byteItem in byteList)
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
                            uploadedRecord.Add(new LoanObj
                            {
                                Excel_line_number = i,
                                CustomerEmail = workSheet.Cells[i, 1].Value.ToString() != null ? workSheet.Cells[i, 1].Value.ToString() : "",
                                LoanReferenceNumber = workSheet.Cells[i, 2].Value.ToString() != null ? workSheet.Cells[i, 2].Value.ToString() : "",
                                ProductName = workSheet.Cells[i, 3].Value.ToString() != null ? workSheet.Cells[i, 3].Value.ToString() : "",
                                CurrencyName = workSheet.Cells[i, 4].Value.ToString() != null ? workSheet.Cells[i, 4].Value.ToString() : "",
                                EffectiveDate = Convert.ToDateTime(workSheet.Cells[i, 5].Value) != null ? Convert.ToDateTime(workSheet.Cells[i, 5].Value.ToString()) : DateTime.Now,
                                OutstandingTenor = Convert.ToInt32(workSheet.Cells[i, 6].Value) != 0 ? Convert.ToInt32(workSheet.Cells[i, 6].Value) : 0,
                                FirstPrincipalPaymentDate = Convert.ToDateTime(workSheet.Cells[i, 7].Value) != null ? Convert.ToDateTime(workSheet.Cells[i, 7].Value.ToString()) : DateTime.Now,
                                FirstInterestPaymentDate = Convert.ToDateTime(workSheet.Cells[i, 8].Value) != null ? Convert.ToDateTime(workSheet.Cells[i, 8].Value.ToString()) : DateTime.Now,
                                OutstandingPrincipal = Convert.ToDecimal(workSheet.Cells[i, 9].Value) != 0 ? Convert.ToDecimal(workSheet.Cells[i, 9].Value.ToString()) : 0,
                                OutstandingInterest = Convert.ToDecimal(workSheet.Cells[i, 10].Value) != 0 ? Convert.ToDecimal(workSheet.Cells[i, 10].Value.ToString()) : 0,
                                InterestFrequencyTypeName = workSheet.Cells[i, 11].Value.ToString() != null ? workSheet.Cells[i, 11].Value.ToString() : "",
                                PricipalFrequencyTypeName = workSheet.Cells[i, 12].Value.ToString() != null ? workSheet.Cells[i, 12].Value.ToString() : "",
                                CompanyName = workSheet.Cells[i, 13].Value.ToString() != null ? workSheet.Cells[i, 13].Value.ToString() : "",
                            });
                        }
                    }
                }

                var getCurrencyList = await _serverRequest.GetCurrencyAsync();
                var getCompanyList = await _serverRequest.GetAllCompanyAsync();

                if (uploadedRecord.Count > 0)
                {
                    var oldUploadedLoans = _dataContext.credit_loan.Where(x => x.IsUploaded == true).ToList();
                    if (oldUploadedLoans.Count() > 0)
                    {
                        _dataContext.credit_loan.RemoveRange(oldUploadedLoans);
                        await _dataContext.SaveChangesAsync();
                    }                  
                    foreach (var entity in uploadedRecord)
                    {
                        var oldLoan = _dataContext.credit_loan.FirstOrDefault(x => x.LoanRefNumber == entity.LoanReferenceNumber);
                        if (oldLoan != null)
                        {
                            var oldSchedule = _dataContext.credit_loanscheduleperiodic.Where(x => x.LoanId == oldLoan.LoanId).ToList();
                            if (oldSchedule.Count() > 0)
                            {
                                _dataContext.credit_loanscheduleperiodic.RemoveRange(oldSchedule);
                            }
                            var oldLoanApplication = _dataContext.credit_loanapplication.FirstOrDefault(x => x.LoanApplicationId == oldLoan.LoanApplicationId);
                            if (oldLoanApplication != null)
                            {
                                _dataContext.credit_loanapplication.Remove(oldLoanApplication);
                            }
                            _dataContext.credit_loan.Remove(oldLoan);
                            await _dataContext.SaveChangesAsync();
                        }
                        if (entity.CustomerEmail == "" || entity.CustomerEmail == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Please include a valid customer's email detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        var accountTypeExist = _dataContext.credit_loancustomer.Where(x => x.Email == entity.CustomerEmail).FirstOrDefault();
                        if (accountTypeExist == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Customer Email {entity.CustomerEmail} doesn't match any user, detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        var currency = getCurrencyList.commonLookups.Where(x => x.LookupName.ToLower().Trim() == entity.CurrencyName.ToLower().Trim()).FirstOrDefault();
                        if (currency == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Please include a valid currency name, detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        var company = getCompanyList.companyStructures.Where(x => x.name.ToLower().Trim() == entity.CompanyName.ToLower().Trim()).FirstOrDefault();
                        if (company == null)
                        {
                            response.Status.Message.FriendlyMessage = $"The Company name " + entity.CompanyName + " doesn't exist on the application, detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        var product = _dataContext.credit_product.Where(x => x.ProductName.ToLower().Trim() == entity.ProductName.ToLower().Trim()).FirstOrDefault();
                        if (product == null)
                        {
                            response.Status.Message.FriendlyMessage = $"The product name " + entity.ProductName + " doesn't exist on the application, detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        var freqPrincipal = _dataContext.credit_frequencytype.Where(x => x.Mode.ToLower().Trim() == entity.PricipalFrequencyTypeName.ToLower().Trim()).FirstOrDefault();
                        if (freqPrincipal == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Please include a valid principal frequency name, detected on line {entity.Excel_line_number}";
                            return response;
                        }

                        var freqInterest = _dataContext.credit_frequencytype.Where(x => x.Mode.ToLower().Trim() == entity.PricipalFrequencyTypeName.ToLower().Trim()).FirstOrDefault();
                        if (freqInterest == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Please include a valid interest frequency name, detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        if (entity.EffectiveDate == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Please Enter Effective Date, detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        if (entity.FirstInterestPaymentDate == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Please Enter First Interest PaymentDate, detected on line {entity.Excel_line_number}";
                            return response;
                        }

                        if (entity.FirstPrincipalPaymentDate == null)
                        {
                            response.Status.Message.FriendlyMessage = $"Please Enter First Interest PaymentDate, detected on line {entity.Excel_line_number}";
                            return response;
                        }
                        entity.CustomerId = accountTypeExist.CustomerId;

                        if (entity.OutstandingTenor == 0)//Bad loans
                        {
                            var loanApplicationId = UpdateLoanApplication(product.ProductId, entity);                           
                            var loan = new credit_loan
                            {
                                CustomerId = accountTypeExist.CustomerId,
                                ProductId = product.ProductId,
                                LoanApplicationId = loanApplicationId,
                                ScheduleTypeId = product.ScheduleTypeId,
                                CurrencyId = currency.LookupId,
                                PrincipalFrequencyTypeId = freqPrincipal.FrequencyTypeId,
                                InterestFrequencyTypeId = freqInterest.FrequencyTypeId,
                                ApprovalStatusId = (int)ApprovalStatus.Approved,
                                ApprovedDate = entity.ApprovedDate,
                                BookingDate = DateTime.Now,
                                EffectiveDate = entity.EffectiveDate,
                                MaturityDate = entity.MaturityDate,
                                LoanStatusId = entity.LoanStatusId,
                                IntegralFeeAmount = 0,
                                IsDisbursed = true,
                                IsUploaded = false,
                                staffEmail = staffEmail,
                                CompanyId = company.companyStructureId,
                                LoanRefNumber = entity.LoanReferenceNumber,
                                PrincipalAmount = entity.OutstandingPrincipal,
                                FirstPrincipalPaymentDate = entity.FirstPrincipalPaymentDate,
                                FirstInterestPaymentDate = entity.FirstInterestPaymentDate,
                                OutstandingPrincipal = entity.OutstandingPrincipal,
                                OutstandingInterest = entity.OutstandingInterest,
                                LoanOperationId = (int)OperationsEnum.LoanBookingApproval,
                                AccrualBasis = 2,
                                FirstDayType = 0,
                                PastDueInterest = 0,
                                PastDuePrincipal = 0,
                                InterestRate = Convert.ToDouble(entity.OutstandingInterest),
                                InterestOnPastDueInterest = 0,
                                InterestOnPastDuePrincipal = 0,
                                CasaAccountId = 0,
                                Active = true,
                                Deleted = false,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = DateTime.Now
                            };
                            _dataContext.credit_loan.Add(loan);
                            _dataContext.SaveChanges();

                            credit_loan_past_due pastDueInterest = new credit_loan_past_due();
                            pastDueInterest.LoanId = loan.LoanId;
                            pastDueInterest.PastDueCode = loan.LoanRefNumber;
                            pastDueInterest.CreditAmount = 0;
                            pastDueInterest.Description = "Past Due Entries on Principal as a result of Account not funded";
                            pastDueInterest.DebitAmount = Math.Abs(loan.PrincipalAmount);
                            pastDueInterest.Date = loan.EffectiveDate;
                            pastDueInterest.TransactionTypeId = (byte)LoanTransactionTypeEnum.Principal;
                            pastDueInterest.Parent_PastDueCode = loan.LoanRefNumber;
                            pastDueInterest.ProductTypeId = (int)product.ProductTypeId;
                            pastDueInterest.LateRepaymentCharge = ((_dataContext.credit_product.Where(x => x.ProductId == product.ProductId && x.Deleted == false).FirstOrDefault().LateTerminationCharge ?? 0) / 365) * (double)pastDueInterest.DebitAmount;

                            credit_loan_repayment repayment = new credit_loan_repayment();
                            repayment.LoanId = loan.LoanId;
                            repayment.Date = loan.EffectiveDate;
                            repayment.InterestAmount = loan.OutstandingInterest??0;
                            repayment.PrincipalAmount = loan.PrincipalAmount;
                            repayment.ClosingBalance = 0;
                            var casa = _dataContext.credit_casa.FirstOrDefault(x => x.CustomerId == accountTypeExist.CustomerId);
                            if (casa != null)
                            {
                                casa.AvailableBalance = 0;
                                casa.LedgerBalance = 0;
                            }

                            _dataContext.credit_loan_repayment.Add(repayment);
                            _dataContext.credit_loan_past_due.Add(pastDueInterest);

                            GenerateCustomerTransaction(loan.CustomerId, loan.PrincipalAmount, loan.LoanRefNumber);
                        } 
                        else
                        {
                            if (entity.EffectiveDate >= DateTime.Now)
                            {
                                response.Status.Message.FriendlyMessage = $"Effective date cannot be in the future, detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            entity.MaturityDate = entity.EffectiveDate.AddDays(entity.OutstandingTenor);
                            if (entity.FirstInterestPaymentDate >= entity.MaturityDate)
                            {
                                response.Status.Message.FriendlyMessage = $"First Interest Payment Date must be less than the maturity date, detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            if (entity.FirstPrincipalPaymentDate >= entity.MaturityDate)
                            {
                                response.Status.Message.FriendlyMessage = $"First Principal Payment Date must be less than the maturity date, detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            if (entity.EffectiveDate == entity.FirstPrincipalPaymentDate)
                            {
                                response.Status.Message.FriendlyMessage = $"First Principal Payment Date cannot be equal to the Effective date, detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            if (entity.EffectiveDate == entity.FirstInterestPaymentDate)
                            {
                                response.Status.Message.FriendlyMessage = $"First Interest Payment Date cannot be equal to the Effective date, detected on line {entity.Excel_line_number}";
                                return response;
                            }
                            var loanApplicationId = UpdateLoanApplication(product.ProductId, entity);

                             var loan = new credit_loan
                            {
                                CustomerId = accountTypeExist.CustomerId,
                                LoanApplicationId = loanApplicationId,
                                ProductId = product.ProductId,
                                ScheduleTypeId = product.ScheduleTypeId,
                                CurrencyId = currency.LookupId,
                                PrincipalFrequencyTypeId = freqPrincipal.FrequencyTypeId,
                                InterestFrequencyTypeId = freqInterest.FrequencyTypeId,
                                ApprovalStatusId = (int)ApprovalStatus.Approved,
                                ApprovedDate = entity.ApprovedDate,
                                BookingDate = DateTime.Now,
                                EffectiveDate = entity.EffectiveDate,
                                MaturityDate = entity.MaturityDate,
                                LoanStatusId = entity.LoanStatusId,
                                IntegralFeeAmount = 0,
                                IsDisbursed = true,
                                IsUploaded = true,
                                staffEmail = staffEmail,
                                CompanyId = company.companyStructureId,
                                LoanRefNumber = entity.LoanReferenceNumber,
                                PrincipalAmount = entity.OutstandingPrincipal,
                                FirstPrincipalPaymentDate = entity.FirstPrincipalPaymentDate,
                                FirstInterestPaymentDate = entity.FirstInterestPaymentDate,
                                OutstandingPrincipal = entity.OutstandingPrincipal,
                                OutstandingInterest = entity.OutstandingInterest,
                                LoanOperationId = (int)OperationsEnum.LoanBookingApproval,
                                AccrualBasis = 2,
                                FirstDayType = 0,
                                PastDueInterest = 0,
                                PastDuePrincipal = 0,
                                InterestRate = Convert.ToDouble(entity.OutstandingInterest),
                                InterestOnPastDueInterest = 0,
                                InterestOnPastDuePrincipal = 0,
                                CasaAccountId = 0,
                                Active = true,
                                Deleted = false,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = DateTime.Now
                            };
                            loans.Add(loan);
                        }
                    }
                }

                _dataContext.credit_loan.AddRange(loans);
                await _dataContext.SaveChangesAsync();

                //var loan_generation_reponse = await GenerateScheduleForUploadedLoans();
                //if (loan_generation_reponse != "success")
                //{
                //    response.Status.Message.FriendlyMessage = loan_generation_reponse;
                //    return response;
                //}

                response.Status.Message.FriendlyMessage = $"Records Uploaded Successfully";
                response.Status.IsSuccessful = true;
                return response;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = ex?.Message, TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        private int UpdateLoanApplication(int productId, LoanObj entity)
        {
            var product = _dataContext.credit_product.Find(productId);
            var refNo = GenerateLoanReferenceNumber();

            var loanApplication = new credit_loanapplication
            {
                ApprovedAmount = entity.OutstandingPrincipal,
                ApprovedProductId = product.ProductId,
                ApprovedRate = product.Rate,
                ApprovedTenor = product.TenorInDays ?? 0,
                CurrencyId = entity.CurrencyId,
                CustomerId = entity.CustomerId,
                EffectiveDate = entity.EffectiveDate,
                FirstInterestDate = entity.FirstInterestPaymentDate,
                FirstPrincipalDate = entity.FirstPrincipalPaymentDate,
                ExchangeRate = entity.ExchangeRate,
                HasDoneChecklist = true,
                MaturityDate = entity.MaturityDate,
                ProposedAmount = entity.OutstandingPrincipal,
                ProposedProductId = product.ProductId,
                ProposedRate = product.Rate,
                ProposedTenor = product.TenorInDays ?? 0,
                LoanApplicationStatusId = (int)LoanApplicationStatusEnum.OfferLetterGenerationCompleted,
                ApplicationRefNumber = refNo,
                ApplicationDate = DateTime.Now.Date,
                ApprovalStatusId = 2,
                Active = true,
                Deleted = false,
                CreatedBy = entity.CreatedBy,
                CreatedOn = DateTime.Now
            };
            _dataContext.credit_loanapplication.Add(loanApplication);
            _dataContext.SaveChanges();
            return loanApplication.LoanApplicationId;
        }

        public void GenerateCustomerTransaction(int customerId, decimal PrincipalAmount, string LoanRefNumber)
        {
            var _customer = _dataContext.credit_loancustomer.Find(customerId);
            CustomerTransactionObj loanDisbursementCustomerEntry = new CustomerTransactionObj
            {
                CasaAccountNumber = _customer.CASAAccountNumber,
                Description = "Loan Disbursement",
                TransactionDate = DateTime.Now,
                ValueDate = DateTime.Now,
                TransactionType = "Debit",
                CreditAmount = 0,
                DebitAmount = PrincipalAmount,
                Beneficiary = _customer.FirstName + " " + _customer.LastName,
                ReferenceNo = LoanRefNumber,
            };
            _customerTrans.CustomerTransaction(loanDisbursementCustomerEntry);
        }

        private string GenerateLoanReferenceNumber()
        {
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
            TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.Ticks) - epochTicks;
            double unixTime = (int)unixTicks.TotalSeconds;
            return unixTime.ToString();
        }

        //[HttpGet(ApiRoutes.Loan.UPLOAD_GENERATE_SCHEDULE_LOANS)]
        public async Task GenerateScheduleForUploadedLoans() 
        {
            //var response = new LoanRespObj { Status = new APIResponseStatus { Message = new APIResponseMessage() } };
            try
            {
                var user =  await _serverRequest.UserDataAsync();
                var loanList = await (from a in _dataContext.credit_loan
                                where a.Deleted == false && a.IsUploaded == true select a).ToListAsync();

                foreach (var item in loanList)
                {
                    var customer = await _dataContext.credit_loancustomer.FindAsync(item.CustomerId);
                    var loan = await _dataContext.credit_loan.FindAsync(item.LoanId);
                    var product = await _dataContext.credit_product.FindAsync(item.ProductId);
                    try
                    {
                        var applicationResponse = await _repo.Disburse_Loan_By_Upload(item.LoanId);
                        //Update CustomerTransaction
                        //if (applicationResponse.DisbursementEntry != null && applicationResponse.IntegralFeeEntry != null)
                        //{
                        //    await Task.Run(() => _customerTrans.CustomerTransaction(applicationResponse.DisbursementEntry));
                        //    await Task.Run(()=>_customerTrans.CustomerTransaction(applicationResponse.IntegralFeeEntry));
                        //}

                        //Generate Schedule
                        if (applicationResponse.loanPayment != null && applicationResponse.AnyIdentifier > 0)
                        {
                            await _schedule.AddLoanSchedule(applicationResponse.AnyIdentifier, applicationResponse.loanPayment);
                        }

                        loan.IsUploaded = false;
                        await  _dataContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        var msg = $"Hi {user.StaffName}, <br> The loan schedule generation was not successful for the uploaded loans due to the following reasons: <br/>" +
                            $"{ex.Message}. <br/> This happened on the loan with product name of {product.ProductName} and customer with the email of {customer.Email}";
                        await _serverRequest.SendMail(new MailObj
                        {
                            fromAddresses = new List<FromAddress> { },
                            toAddresses = new List<ToAddress>
                        {
                            new ToAddress{ address = user.Email, name = user.StaffName}
                        },
                            subject = "Error Occured while on Loan Schedule Generation",
                            content = msg,
                            sendIt = false,
                            saveIt = true,
                            module = 2,
                            userIds = user.UserId
                        });

                        //return "failure";
                        //throw;
                    }
                  
                }
             
                await _serverRequest.SendMail(new MailObj
                {
                    fromAddresses = new List<FromAddress> { },
                    toAddresses = new List<ToAddress>
                        {
                            new ToAddress{ address = user.Email, name = user.StaffName}
                        },
                    subject = "Successful Loan Schedule Generation",
                    content = $"Hi {user.StaffName}, <br> The loan schedule generation was successful for the uploaded loans",
                    sendIt = false,
                    saveIt = true,
                    module = 2,
                    userIds = user.UserId
                });
                //response.Status.Message.FriendlyMessage = $"Schedule Uploaded Successfully";
                //response.Status.IsSuccessful = true;
                //return response;
                //return "success";
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
            }
            //response.Status.Message.FriendlyMessage = $"Schedule Uploaded Successfully";
            //response.Status.IsSuccessful = true;
            //return response;
            //return "success";
        }


        [HttpGet(ApiRoutes.Loan.DOWNLOAD_LOANS)]
        public async Task<ActionResult<LoanRespObj>> GenerateExportLoanCustomer()
        {
            try
            {
                var response = await _repo.GenerateExportLoan();

                return new LoanRespObj
                {
                    Export = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_REVIEWED_LOANS)]
        public async Task<ActionResult<LoanApplicationRespObj>> GetAllLoanApplicationOfferLetterReviewed()
        {
            try
            {
                var response = _repo.GetAllLoanApplicationOfferLetterReviewed();
                return new LoanApplicationRespObj
                {
                    LoanApplications = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanApplicationRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_REVIEWED_LOANS_ID)]
        public async Task<ActionResult<LoanApplicationRespObj>> GetAllLoanApplicationOfferLetterReviewedById([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = _repo.GetAllLoanApplicationOfferLetterReviewedById(model.LoanApplicationId);
                return new LoanApplicationRespObj
                {
                    LoanApplications = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanApplicationRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_ALL_LOANS)]
        public async Task<ActionResult<CreditLoanRespObj>> GetAllCreditLoan()
        {
            try
            {
                var response = _repo.GetAllCreditLoan();
                return Ok(new CreditLoanRespObj
                {
                    Loans = _mapper.Map<List<credit_loan_obj>>(response),
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = true,
                        Message = new APIResponseMessage { FriendlyMessage = response.Count() > 0 ? "Search record found" : "No record found" }
                    },
                 });
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new CreditLoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_DETAILED_INFORMATION)]
        public async Task<ActionResult<LoanRespObj>> GetLoanDetailInformation([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = await _repo.GetLoanDetailInformation(model.LoanId);
                return new LoanRespObj
                {
                    LoanDetail = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_SCHEDULE_INPUT)]
        public async Task<ActionResult<LoanRespObj>> GetScheduleInput([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = _repo.GetScheduleInput(model.LoanApplicationId);
                return new LoanRespObj
                {
                    LoanSchedule = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_PAST_DUE_INFORMATION)]
        public async Task<ActionResult<LoanRespObj>> GetPastDueLoanInformation()
        {
            try
            {
                var response = _repo.GetPastDueLoanInformation();
                return new LoanRespObj
                {
                   ManageLoans = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_PAYMENT_DUE_INFORMATION)]
        public async Task<ActionResult<LoanRespObj>> GetPaymentDueLoanInformation()
        {
            try
            {
                var response = _repo.GetPaymentDueLoanInformation();
                return new LoanRespObj
                {
                    ManageLoans = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_MANAGED_INFORMATION)]
        public async Task<ActionResult<LoanRespObj>> GetManagedLoanInformation([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = _repo.GetManagedLoanInformation(model.LoanId);

                return new LoanRespObj
                {
                    ManageLoans = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_BOOKING_APPROVAL_LIST)]
        public async Task<ActionResult<LoanRespObj>> GetLoanApplicationForAppraisalAsync()
        {
            try
            {
                var user = await _serverRequest.UserDataAsync();
                return await _repo.GetLoanBookingAwaitingApproval(user.UserName);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.UPDATE_LOAN_REPAYMENT_BY_FLUTTERWAVE)]
        public async Task<LoanRepaymentRespObj> RepaymentWithFlutterWave([FromBody]repaymentObj model)
        {
            try
            {
                var res = _repo.repaymentWithFlutterWave(model);
                if (res.customerTrans != null)
                {
                    _customerTrans.CustomerTransaction(res.customerTrans);
                }
                return res;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRepaymentRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.UPDATE_LOAN_REPAYMENT_BY_FLUTTERWAVE_ZERO)]
        public async Task<LoanRepaymentRespObj> repaymentZeroWithFlutterWave([FromBody]repaymentObj model)
        {
            try
            {
                var res = _repo.repaymentZeroWithFlutterWave(model);
                if (res.customerTrans != null)
                {
                    _customerTrans.CustomerTransaction(res.customerTrans);
                }
                return res;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRepaymentRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.LOAN_BOOKING_APPROVAL)]
        public async Task<ActionResult<ApprovalRegRespObj>> GoForApproval([FromBody] ApprovalObj entity)
        {
            using (var _trans = _dataContext.Database.BeginTransaction())
            {
                try
                {
                    var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;
                    var user = await _serverRequest.UserDataAsync();

                    var loan = _dataContext.credit_loan.Find(entity.TargetId);
                    if (user.Staff_limit < loan.PrincipalAmount)
                    {
                        return new ApprovalRegRespObj { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "The amount you're trying to approve is beyond your staff limit" } } };
                    }

                    var req = new IndentityServerApprovalCommand
                    {
                        ApprovalComment = entity.Comment,
                        ApprovalStatus = entity.ApprovalStatusId,
                        TargetId = entity.TargetId,
                        WorkflowToken = loan.WorkflowToken,
                        ReferredStaffId = entity.ReferredStaffId
                    };

                    var previousDetails = _dataContext.cor_approvaldetail.Where(x => x.WorkflowToken.Contains(loan.WorkflowToken) && x.TargetId == entity.TargetId).ToList();
                    var lastDate = loan.CreatedOn;
                    if (previousDetails.Count() > 0)
                    {
                        lastDate = previousDetails.OrderByDescending(x => x.ApprovalDetailId).FirstOrDefault().Date;
                    }
                    if (previousDetails.Count() > 0)
                    {
                        lastDate = previousDetails.OrderByDescending(x => x.ApprovalDetailId).FirstOrDefault().Date;
                    }
                    var details = new cor_approvaldetail
                    {
                        Comment = entity.Comment,
                        Date = DateTime.Now,
                        ArrivalDate = previousDetails.Count() > 0 ? lastDate : loan.CreatedOn,
                        StatusId = entity.ApprovalStatusId,
                        TargetId = entity.TargetId,
                        StaffId = user.StaffId,
                        WorkflowToken = loan.WorkflowToken
                    };

                    var result = await _serverRequest.StaffApprovalRequestAsync(req);

                    if (!result.IsSuccessStatusCode)
                    {
                        return new ApprovalRegRespObj
                        {
                            Status = new APIResponseStatus
                            {
                                Message = new APIResponseMessage { FriendlyMessage = result.ReasonPhrase }
                            }
                        };
                    }

                    var stringData = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<ApprovalRegRespObj>(stringData);

                    if (!response.Status.IsSuccessful)
                    {
                        return new ApprovalRegRespObj
                        {
                            Status = response.Status
                        };
                    }

                    if (response.ResponseId == (int)ApprovalStatus.Processing)
                    {
                        loan.ApprovalStatusId = (int)ApprovalStatus.Processing;
                        await _dataContext.cor_approvaldetail.AddAsync(details);
                        await _dataContext.SaveChangesAsync();
                        _trans.Commit();
                        return new ApprovalRegRespObj
                        {
                            ResponseId = (int)ApprovalStatus.Processing,
                            Status = new APIResponseStatus { IsSuccessful = true, Message = response.Status.Message }
                        };
                    }

                    if (response.ResponseId == (int)ApprovalStatus.Revert)
                    {
                        loan.ApprovalStatusId = (int)ApprovalStatus.Revert;
                        await _dataContext.cor_approvaldetail.AddAsync(details);
                        await _dataContext.SaveChangesAsync();
                        _trans.Commit();
                        return new ApprovalRegRespObj
                        {
                            ResponseId = (int)ApprovalStatus.Revert,
                            Status = new APIResponseStatus { IsSuccessful = true, Message = response.Status.Message }
                        };
                    }

                    if (response.ResponseId == (int)ApprovalStatus.Approved)
                    {
                        var applicationResponse = _repo.DisburseLoan(entity.TargetId, user.StaffId, user.UserName, entity.Comment);

                        //Update CustomerTransaction
                        if (applicationResponse.DisbursementEntry != null && applicationResponse.IntegralFeeEntry != null)
                        {
                            _customerTrans.CustomerTransaction(applicationResponse.DisbursementEntry);
                            _customerTrans.CustomerTransaction(applicationResponse.IntegralFeeEntry);
                            if (applicationResponse.NonIntegralFeeEntry != null)
                            {
                                _customerTrans.CustomerTransaction(applicationResponse.NonIntegralFeeEntry);
                            }
                        }

                        //Generate Schedule
                        if (applicationResponse.loanPayment != null && applicationResponse.AnyIdentifier > 0)
                        {
                            await _schedule.AddLoanSchedule(applicationResponse.AnyIdentifier, applicationResponse.loanPayment);
                        }

                        //Update IFRS
                        _ifrs.UpdateScoreCardHistoryByLoanDisbursement(entity.TargetId, user.UserId);

                        //Pay with Flutterwave
                        if (applicationResponse.FlutterObj != null)
                        {
                            var flutter = _serverRequest.GetFlutterWaveKeys().Result;
                            if (flutter.keys.useFlutterWave)
                            {
                                var res = _flutter.createBulkTransfer(applicationResponse.FlutterObj).Result;
                                loan.ApprovalStatusId = (int)ApprovalStatus.Approved;
                                await _dataContext.cor_approvaldetail.AddAsync(details);
                                await _dataContext.SaveChangesAsync();
                                _trans.Commit();
                                if (res.status.ToLower().Trim() != "success")
                                {
                                    return new ApprovalRegRespObj
                                    {
                                        ResponseId = (int)ApprovalStatus.Revert,
                                        Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Loan disbursed successfully but transfer creation failed" } }
                                    };
                                }
                                else if (res.status.ToLower().Trim() == "success")
                                {
                                    return new ApprovalRegRespObj
                                    {
                                        ResponseId = (int)ApprovalStatus.Revert,
                                        Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Loan disbursed successfully and transfer creation successful" } }
                                    };
                                }
                            }
                        }
                        loan.ApprovalStatusId = (int)ApprovalStatus.Approved;
                        await _dataContext.cor_approvaldetail.AddAsync(details);
                        await _dataContext.SaveChangesAsync();
                        _trans.Commit();

                        var customer = _dataContext.credit_loancustomer.Find(loan.CustomerId);
                        await _serverRequest.SendMail(new MailObj
                        {
                            fromAddresses = new List<FromAddress> { },
                            toAddresses = new List<ToAddress>
                            {
                                new ToAddress{ address = customer.Email, name = customer.FirstName}
                            },
                            subject = "Loan Successfully Disbursed",
                            content = $"Hi {customer.FirstName}, <br> Your loan application has been finally disbursed. <br/> Loan Amount : {loan.PrincipalAmount}",
                            sendIt = true,
                            saveIt = true,
                            module = 2,
                            userIds = customer.UserIdentity
                        });

                        await _serverRequest.SendMail(new MailObj
                        {
                            fromAddresses = new List<FromAddress> { },
                            toAddresses = new List<ToAddress>
                            {
                                new ToAddress{ address = customer.Email, name = customer.FirstName}
                            },
                            subject = "Loan Fee Charge",
                            content = $"Hi {customer.FirstName}, <br> Fee charge payment on {loan.LoanRefNumber} has been debited into your operating account.. <br/> Fee Amount : {applicationResponse.IntegralFeeEntry.Amount}",
                            sendIt = true,
                            saveIt = true,
                            module = 2,
                            userIds = customer.UserIdentity
                        });
                        return new ApprovalRegRespObj
                        {
                            ResponseId = (int)ApprovalStatus.Approved,
                            Status = new APIResponseStatus { IsSuccessful = true, Message = response.Status.Message }
                        };
                    }

                    if (response.ResponseId == (int)ApprovalStatus.Disapproved)
                    {
                        loan.ApprovalStatusId = (int)ApprovalStatus.Disapproved;
                        await _dataContext.cor_approvaldetail.AddAsync(details);
                        await _dataContext.SaveChangesAsync();
                        _trans.Commit();
                        return new ApprovalRegRespObj
                        {
                            ResponseId = (int)ApprovalStatus.Disapproved,
                            Status = new APIResponseStatus { IsSuccessful = true, Message = response.Status.Message }
                        };
                    }

                    return new ApprovalRegRespObj
                    {
                        Status = response.Status
                    };
                }
                catch (Exception ex)
                {
                    _trans.Rollback();
                    var errorCode = ErrorID.Generate(5);
                    _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                    return new ApprovalRegRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                    };
                }
                finally { _trans.Dispose(); }
            }
        }

        #endregion

        #region LOAN COMMENT
        [HttpPost(ApiRoutes.Loan.ADD_LOAN_COMMENT)]
        public async Task<ActionResult<LoanRespObj>> UpdateCreditLoanComment([FromBody] CreditLoanCommentObj model)
        {
            try
            {
                var identity = await _serverRequest.UserDataAsync();
                var user = identity.UserName;

                model.CreatedBy = user;
                model.UpdatedBy = user;

                var response = _repo.UpdateCreditLoanComment(model);
                if (response)
                {
                    return new LoanRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = $"Record saved successfully." } }
                    };
                }
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Record not saved" } }
                };

            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_ALL_LOAN_COMMENT)]
        public async Task<ActionResult<LoanCommentRespObj>> GetAllCreditLoanComment([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = _repo.GetAllCreditLoanComment(model.LoanId, model.LoanScheduleId);
                return new LoanCommentRespObj
                {
                    LoanComments = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanCommentRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_COMMENT_ID)]
        public async Task<ActionResult<LoanCommentRespObj>> GetCreditLoanComment([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = _repo.GetCreditLoanComment(model.LoanId, model.LoanScheduleId, model.LoanCommentId);
                var respList = new List<CreditLoanCommentObj> { response };
                return new LoanCommentRespObj
                {
                    LoanComments = respList,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanCommentRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.DELETE_LOAN_COMMENT)]
        public ActionResult<LoanApplicationRespObj> DeleteCreditLoanComment([FromBody] DeleteLoanCommand command)
        {
            var response = false;
            var Ids = command.Ids;
            foreach (var id in Ids)
            {
                response = _repo.DeleteCreditLoanComment(id);
            }
            if (!response)
                return BadRequest(
                    new DeleteRespObj
                    {
                        Deleted = false,
                        Status = new APIResponseStatus { Message = new APIResponseMessage { FriendlyMessage = "Unsuccessful" } }
                    });
            return Ok(
                    new DeleteRespObj
                    {
                        Deleted = true,
                        Status = new APIResponseStatus { Message = new APIResponseMessage { FriendlyMessage = "Successful" } }
                    });
        }
        #endregion

        #region LOAN DECISION
        [HttpPost(ApiRoutes.Loan.ADD_LOAN_DECISION)]
        public async Task<ActionResult<LoanRespObj>> UpdateCreditLoanDecision([FromBody] CreditLoanDecisionObj model)
        {
            try
            {
                var identity = await _serverRequest.UserDataAsync();
                var user = identity.UserName;

                model.CreatedBy = user;
                model.UpdatedBy = user;

                var response = _repo.UpdateCreditLoanDecision(model);
                if (response)
                {
                    return new LoanRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = $"Record saved successfully." } }
                    };
                }
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Record not saved" } }
                };

            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_ALL_LOAN_DECISION)]
        public async Task<ActionResult<LoanCommentRespObj>> GetAllCreditLoanDecision([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = _repo.GetAllCreditLoanDecision(model.LoanId, model.LoanScheduleId);
                return new LoanCommentRespObj
                {
                    LoanDecisions = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanCommentRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_LOAN_DECISION_ID)]
        public async Task<ActionResult<LoanCommentRespObj>> GetCreditLoanDecision([FromQuery] LoanSearchObj model)
        {
            try
            {
                var response = _repo.GetCreditLoanDecision(model.LoanId, model.LoanScheduleId);
                var respList = new List<CreditLoanDecisionObj> { response };
                return new LoanCommentRespObj
                {
                    LoanDecisions = respList,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanCommentRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.DELETE_LOAN_DECISION)]
        public ActionResult<LoanApplicationRespObj> DeleteCreditLoanDecision([FromBody] DeleteLoanCommand command)
        {
            var response = false;
            var Ids = command.Ids;
            foreach (var id in Ids)
            {
                response = _repo.DeleteCreditLoanDecision(id);
            }
            if (!response)
                return BadRequest(
                    new DeleteRespObj
                    {
                        Deleted = false,
                        Status = new APIResponseStatus { Message = new APIResponseMessage { FriendlyMessage = "Unsuccessful" } }
                    });
            return Ok(
                    new DeleteRespObj
                    {
                        Deleted = true,
                        Status = new APIResponseStatus { Message = new APIResponseMessage { FriendlyMessage = "Successful" } }
                    });
        }
        #endregion

        #region LOAN CHEQUES

        [HttpPost(ApiRoutes.Loan.ADD_LOAN_CHEQUE)]
        public async Task<ActionResult<LoanChequeRespObj>> AddUpdateLoanCheque()
        {
            try
            {
                var postedFile = _httpContextAccessor.HttpContext.Request.Form.Files;
                var end = Convert.ToString(_httpContextAccessor.HttpContext.Request.Form["end"]);
                var loanChequeId = Convert.ToInt32(_httpContextAccessor.HttpContext.Request.Form["loanChequeId"]);
                var loanId = Convert.ToInt32(_httpContextAccessor.HttpContext.Request.Form["loanApplicationId"]);
                var start = Convert.ToString(_httpContextAccessor.HttpContext.Request.Form["start"]);

                var byteArray = new byte[0];
                foreach (var fileBit in postedFile)
                {
                    if (fileBit.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileBit.CopyToAsync(ms);
                            byteArray = ms.ToArray();
                        }
                    }
                }

                var user = await _serverRequest.UserDataAsync();
                var createdBy = user.UserName;

                var model = new loan_cheque_obj
                {
                    LoanChequeId = loanChequeId,
                    LoanId = loanId,
                    GeneralUpload = byteArray,
                    End = end,
                    Start = start,
                    CreatedBy = createdBy
                };

                var response = _repo.AddUpdateLoanCheque(model);

                    return new LoanChequeRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = response ? true : false, Message = new APIResponseMessage { FriendlyMessage = response ? $"Successful" : "Unsuccessful" } }
                    };

            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanChequeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.UPLOAD_LOAN_CHEQUE)]
        public async Task<ActionResult<LoanChequeRespObj>> UploadSingleCheque()
        {
            try
            {
                var postedFile = _httpContextAccessor.HttpContext.Request.Form.Files;
                var loanChequeListId = Convert.ToInt32(_httpContextAccessor.HttpContext.Request.Form["loanChequeListId"]);
                var filename = (_httpContextAccessor.HttpContext.Request.Form.Files["singleUpload"]);

                var byteArray = new byte[0];
                foreach (var fileBit in postedFile)
                {
                    if (fileBit.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileBit.CopyToAsync(ms);
                            byteArray = ms.ToArray();
                        }
                    }
                }

                var model = new loan_cheque_obj
                {
                    LoanChequeListId = loanChequeListId,
                    SingleUpload = byteArray,
                    StatusName = filename.FileName,
                };
                return _repo.UploadSingleCheque(model);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanChequeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.UPLOAD_LOAN_CHEQUE_AMOUNT)]
        public async Task<ActionResult<LoanChequeRespObj>> UploadCorporateCustomer()
        {
            try
            {
                var loanId = Convert.ToInt32(_httpContextAccessor.HttpContext.Request.Form["loanapplicationId"]);
                var files = _httpContextAccessor.HttpContext.Request.Form.Files;
                var byteList = new List<byte[]>();
                foreach (var fileBit in files)
                {
                    if (fileBit.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileBit.CopyToAsync(ms);
                            byteList.Add(ms.ToArray());
                        }
                    }
                }
                return await _repo.UploadChequeAmount(byteList, loanId);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanChequeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = ex?.Message, TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.UPDATE_LOAN_CHEQUE_STATUS)]
        public async Task<ActionResult<LoanChequeRespObj>> UpdateChequeStatus([FromBody] loan_cheque_obj model)
        {
            try
            {
                var response = _repo.UpdateChequeStatus(model);
                if (response.Status.IsSuccessful)
                {
                    var _loanapplication = _dataContext.credit_loanapplication.Find(model.LoanId);
                    var _loan = _dataContext.credit_loan.FirstOrDefault(x => x.LoanApplicationId == _loanapplication.LoanApplicationId);
                    var _customer = _dataContext.credit_loancustomer.Find(_loan.CustomerId);
                    CustomerTransactionObj loanDisbursementCustomerEntry = new CustomerTransactionObj
                    {
                        CasaAccountNumber = _customer.CASAAccountNumber,
                        Description = "Loan Repayment by Cheque",
                        TransactionDate = DateTime.Now,
                        ValueDate = DateTime.Now,
                        TransactionType = "Credit",
                        CreditAmount = response.Amount,
                        DebitAmount = 0,
                        Beneficiary = $"{ _customer.FirstName} { _customer.LastName}",
                        ReferenceNo = _loan.LoanRefNumber,
                    };
                    _customerTrans.CustomerTransaction(loanDisbursementCustomerEntry);
                }
                return response;
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanChequeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.UPDATE_LOAN_CHEQUE_AMOUNT)]
        public async Task<ActionResult<LoanChequeRespObj>> UpdateChequeAmount([FromBody] loan_cheque_obj model)
        {
            try
            {
                return _repo.UpdateChequeAmount(model);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanChequeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.Loan.DOWNLOAD_LOAN_CHEQUE)]
        public async Task<ActionResult<LoanChequeRespObj>> DownloadCheque([FromBody] loan_cheque_obj model)
        {
            try
            {
                return _repo.DownloadCheque(model);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanChequeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Loan.GET_ALL_LOAN_CHEQUE)]
        public async Task<ActionResult<LoanChequeRespObj>> GetAllLoanChequeList([FromQuery] LoanSearchObj model)
        {
            try
            {
                return _repo.GetAllLoanChequeList(model.LoanApplicationId);
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new LoanChequeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }
        #endregion
    }
}



