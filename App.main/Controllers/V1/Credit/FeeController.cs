using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Banking.AuthHandler.Interface;
using Banking.Contracts.Response.Approvals;
using Banking.Contracts.Response.Deposit;
using Banking.Contracts.Response.Finance;
using Banking.Contracts.Response.Mail;
using Banking.Contracts.V1;
using Banking.Data;
using Banking.DomainObjects.Credit;
using Banking.Handlers.Auths;
using Banking.Repository.Interface.Credit;
using Banking.Requests;
using GODP.Entities.Models;
using GOSLibraries;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Banking.Contracts.Response.Credit.FeeObjs;
using static Banking.Contracts.Response.Credit.LoanScheduleObjs;
using DeleteRespObj = Banking.Contracts.Response.Deposit.DeleteRespObj;

namespace Banking.Controllers.V1.Credit
{
    [ERPAuthorize]
    public class FeeController : Controller
    {
        private readonly IFeeRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerService _logger;
        private readonly IIdentityServerRequest _identityServer;
        private readonly DataContext _dataContext;
        private readonly ICreditAppraisalRepository _customerTrans;
        public FeeController(IFeeRepository repo, DataContext dataContext, ICreditAppraisalRepository customerTrans, IMapper mapper, IIdentityServerRequest identityServer, IHttpContextAccessor httpContextAccessor, ILoggerService logger)
        {
            _repo = repo;
            _mapper = mapper;
            _identityServer = identityServer;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _customerTrans = customerTrans;
            _dataContext = dataContext;
        }

        

        [HttpGet(ApiRoutes.Fee.GET_ALL_FEE)]
        public async Task<ActionResult<FeeRespObj>> GetAllAccountTypeAsync()
        {
            try
            {
                var response = await _repo.GetAllFeeAsync();
                return new FeeRespObj
                {
                    Fees = _mapper.Map<List<FeeObj>>(response),
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new FeeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }


        [HttpGet(ApiRoutes.Fee.GET_REPAYMENT_TYPE)]
        public async Task<ActionResult<RepaymentTypeRespObj>> GetAllRepaymentTypeAsync()
        {
            try
            {
                var response = await _repo.GetAllRepaymentTypeAsync();
                return new RepaymentTypeRespObj
                {
                    RepaymentType = _mapper.Map<List<RepaymentTypeObj>>(response),
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Successful" } }
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new RepaymentTypeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Fee.GET_ALL_INTEGRAL_FEE)]
        public async Task<ActionResult<FeeRespObj>> GetAllIntegralFeeAsync()
        {
            try
            {
                var response = await _repo.GetAllIntegralFeeAsync();
                return new FeeRespObj
                {
                    Fees = _mapper.Map<List<FeeObj>>(response),
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new FeeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [ERPActivity(Action = UserActions.View, Activity = 81)]
        [HttpGet(ApiRoutes.Fee.GET_FEE_BY_ID)]
        public async Task<ActionResult<FeeRespObj>> GetAccountTypeByIdAsync([FromQuery] FeeSearchObj search)
        {
            try
            {
                if (search.FeeId < 1)
                {
                    return new FeeRespObj
                    {
                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Fee Id is required" } }
                    };
                }

                var response = await _repo.GetFeeByIdAsync(search.FeeId);
                var resplist = new List<credit_fee> { response };
                return new FeeRespObj
                {
                    Fees = _mapper.Map<List<FeeObj>>(resplist),
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new FeeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [ERPActivity(Action = UserActions.View, Activity = 81)]
        [HttpGet(ApiRoutes.Fee.DOWNLOAD_FEE)]
        public async Task<ActionResult<FeeRespObj>> GenerateExportFees()
        {
            try
            {
                var response = _repo.GenerateExportFees();

                return new FeeRespObj
                {
                    export = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new FeeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [ERPActivity(Action = UserActions.Add, Activity = 81)]
        [HttpPost(ApiRoutes.Fee.ADD_FEE)]
        public async Task<ActionResult<FeeRegRespObj>> GetFeeByIdAsync([FromBody] AddUpdateFeeObj model)
        {
            try
            {
                credit_fee item = null;
                if (model.FeeId > 0)
                {
                    item = await _repo.GetFeeByIdAsync(model.FeeId);
                    if (item == null)
                        return new FeeRegRespObj
                        {
                            Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Item does not Exist" } }
                        };
                }

                var domainObj = new credit_fee();
                domainObj.FeeId = model.FeeId > 0 ? model.FeeId : 0;
                domainObj.FeeName = model.FeeName;
                domainObj.IsIntegral = model.IsIntegral;
                domainObj.PassEntryAtDisbursment = model.PassEntryAtDisbursment;
                domainObj.TotalFeeGL = model.TotalFeeGL;
                domainObj.Active = true;
                domainObj.CreatedBy = string.Empty;
                domainObj.CreatedOn = DateTime.Today;
                domainObj.Deleted = false;
                domainObj.UpdatedBy = string.Empty;
                domainObj.UpdatedOn = model.FeeId > 0 ? DateTime.Today : DateTime.Today;

                var isDone = await _repo.AddUpdateFeeAsync(domainObj);
                return new FeeRegRespObj
                {
                    FeeId = domainObj.FeeId,
                    Status = new APIResponseStatus { IsSuccessful = isDone ? true : false, Message = new APIResponseMessage { FriendlyMessage = isDone ? "successful" : "Unsuccessful" } }
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new FeeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [ERPActivity(Action = UserActions.Add, Activity = 81)]
        [HttpPost(ApiRoutes.Fee.UPLOAD_FEE)]
        public async Task<ActionResult<FeeRegRespObj>> UploadFeeAsync()
        {
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

                var user = await _identityServer.UserDataAsync();
                var createdBy = user.UserName;

                var isDone = await _repo.UploadFeeAsync(byteList, createdBy);
                return new FeeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = isDone ? true : false, Message = new APIResponseMessage { FriendlyMessage = isDone ? "Successful" : "Unsuccessful" } }
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new FeeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }


        [ERPActivity(Action = UserActions.Delete, Activity = 81)]
        [HttpPost(ApiRoutes.Fee.DELETE_FEE)]
        public async Task<IActionResult> DeleteBankClosure([FromBody] DeleteFeeCommand command)
        {
            var response = false;
            var Ids = command.FeeIds;
            foreach(var id in Ids)
            {
               response = await _repo.DeleteFeeAsync(id);
            }
            if (!response)
                return BadRequest(
                    new DeleteRespObj
                    {
                        Deleted = false,
                        Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Unsuccessful" } }
                    });
            return Ok(
                    new DeleteRespObj
                    {
                        Deleted = true,
                        Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Successful" } }
                    });
        }

        private void processPastdueLoans(int customerId, decimal repaymentAmount)
        {
            var amount = repaymentAmount;
            var loanList = _dataContext.credit_loan.Where(b=> b.Deleted == false && b.CustomerId == customerId).ToList();
            foreach (var item in loanList)
            {
                var pastdueList = _dataContext.credit_loan_past_due.Where(x => x.LoanId == item.LoanId && x.Deleted == false).ToList();
                if (pastdueList.Count > 0)
                {
                    foreach (var tt in pastdueList)
                    {
                        if (amount >= tt.DebitAmount)
                        {
                            tt.Deleted = true;
                            _dataContext.credit_loan_past_due.Update(tt);
                            amount = amount - tt.DebitAmount;
                        }
                    }
                }
            }
        }


        [HttpPost(ApiRoutes.Fee.ADD_UPDATE_DEPOSITFORM)]
        public async Task<ActionResult<DepositFormRespObj>> AddUpdateDepositForm([FromBody] DepositformObj model)
        {
            try
            {
                var _customer = _dataContext.credit_loancustomer.FirstOrDefault(x => x.CASAAccountNumber == model.AccountNumber);
                processPastdueLoans(_customer.CustomerId, model.Amount);
                var refNo = _repo.AddUpdateDepositForm(model);
                if (refNo != null)
                {                    
                    CustomerTransactionObj CustomerEntry = new CustomerTransactionObj
                    {
                        CasaAccountNumber = model.AccountNumber,
                        Description = model.TransactionParticulars,
                        TransactionDate = DateTime.Now,
                        ValueDate = DateTime.Now,
                        TransactionType = "Credit",
                        CreditAmount = model.Amount,
                        DebitAmount = 0,
                        Beneficiary = _customer.FirstName + " " + _customer.LastName,
                        ReferenceNo = refNo,
                    };
                    _customerTrans.CustomerTransaction(CustomerEntry);

                    deposit_accountsetup operatingAccount = new deposit_accountsetup();
                    operatingAccount = _dataContext.deposit_accountsetup.FirstOrDefault(x => x.DepositAccountId == 3);

                    var FinanceEntry = new TransactionObj
                    {
                        IsApproved = false,
                        CasaAccountNumber = _customer.CASAAccountNumber,
                        CompanyId = model.Structure,
                        Amount = model.Amount,
                        CurrencyId = model.Currency,
                        Description = model.TransactionParticulars,
                        CreditGL = operatingAccount.GLMapping ?? 1,
                        DebitGL = operatingAccount.BankGl ?? 0,
                        ReferenceNo = refNo,
                        OperationId = (int)OperationsEnum.DepositFormSubmit,
                        JournalType = "System",
                        RateType = 1,//Buying Rate
                    };
                    var res = await _identityServer.PassEntryToFinance(FinanceEntry);

                    await _identityServer.SendMail(new MailObj
                    {
                        fromAddresses = new List<FromAddress> { },
                        toAddresses = new List<ToAddress>
                            {
                                new ToAddress{ address = _customer.Email, name = _customer.FirstName}
                            },
                        subject = "Loan Successfully Approved",
                        content = $"Hi {_customer.FirstName}, <br> The payment of {model.Amount} was received in your account {model.AccountNumber} on {DateTime.Now}",
                        sendIt = true,
                        saveIt = true,
                        module = 2,
                        userIds = _customer.UserIdentity
                    });
                }

                return new DepositFormRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage { FriendlyMessage = "Successful" } }
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new DepositFormRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.Fee.GET_ALL_DEPOSITFORM)]
        public async Task<ActionResult<DepositFormRespObj>> GetAllDepositForm()
        {
            try
            {
                var response = _repo.GetAllDepositForm();
                return new DepositFormRespObj
                {
                    DepositForm = response,
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                return new DepositFormRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }
    }
}