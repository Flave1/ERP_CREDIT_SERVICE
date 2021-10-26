using Banking.Contracts.Response.Deposit;
using Banking.Contracts.V1; 
using Banking.Repository.Interface.Deposit;
using AutoMapper;
using GODP.Entities.Models;
using GOSLibraries.GOS_API_Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GOSLibraries;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Banking.AuthHandler.Interface;
using Microsoft.AspNetCore.Http;
using GOSLibraries.GOS_Error_logger.Service;
using Banking.Requests;
using Banking.Handlers.Auths;
using Banking.Data;

namespace Banking.Controllers.V1.Deposit
{
    [ERPAuthorize]
    public class TransactionChargeController : Controller
    {
        private readonly ITransactionChargeService _repo;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerService _logger;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly DataContext _dataContext;
        public TransactionChargeController(
            ITransactionChargeService transactionChargeervice, 
            IMapper mapper, 
            IIdentityService identityService, 
            IHttpContextAccessor httpContextAccessor, 
            ILoggerService logger,
            DataContext dataContext,
            IIdentityServerRequest serverRequest)
        {
            _mapper = mapper;
            _serverRequest = serverRequest;
            _repo = transactionChargeervice;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dataContext = dataContext;
        }

        [HttpGet(ApiRoutes.TransactionCharge.GET_ALL_TRANSACTIONCHARGE)]

        public async Task<ActionResult<TransactionChargeRespObj>> GetAllTransactionChargeAsync()
        {
            try
            {
                var response = await _repo.GetAllTransactionChargeAsync();
                return new TransactionChargeRespObj
                {
                    TransactionCharges = _mapper.Map<List<TransactionChargeObj>>(response),
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                return new TransactionChargeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpGet(ApiRoutes.TransactionCharge.GET_TRANSACTIONCHARGE_BY_ID)]
        public async Task<ActionResult<TransactionChargeRespObj>> GetTransactionChargeByIdAsync([FromQuery] SearchObj search)
        {
            if (search.SearchId < 1)
            {
                return new TransactionChargeRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "TransactionCharge Id is required" } }
                };
            }

            var response = await _repo.GetTransactionChargeByIdAsync(search.SearchId);
            var resplist = new List<deposit_transactioncharge> { response };
            return new TransactionChargeRespObj
            {
                TransactionCharges = _mapper.Map<List<TransactionChargeObj>>(resplist),
            };
        }

        [HttpPost(ApiRoutes.TransactionCharge.ADD_UPDATE_TRANSACTIONCHARGE)]
        public async Task<ActionResult<TransactionChargeRegRespObj>> AddUpDateTransactionCharge([FromBody] AddUpdateTransactionChargeObj model)
        {
            try
            {
                var domainObj = _dataContext.deposit_transactioncharge.Find(model.TransactionChargeId);
                if(domainObj == null)
                    domainObj = new deposit_transactioncharge();

                domainObj.TransactionChargeId = model.TransactionChargeId;
                domainObj.Description = model.Description;
                domainObj.FixedOrPercentage = model.FixedOrPercentage;
                domainObj.Amount_Percentage = model.Amount_Percentage;
                domainObj.Name = model.Name;  

                await _repo.AddUpdateTransactionChargeAsync(domainObj);
                return new TransactionChargeRegRespObj
                {
                    TransactionChargeId = domainObj.TransactionChargeId,
                    Status = new APIResponseStatus { IsSuccessful =  true, Message = new APIResponseMessage { FriendlyMessage = "successful" } }
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                _logger.Error($"ErrorID : {errorCode} Ex : {ex?.Message ?? ex?.InnerException?.Message} ErrorStack : {ex?.StackTrace}");
                return new TransactionChargeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

        [HttpPost(ApiRoutes.TransactionCharge.DELETE_TRANSACTIONCHARGE)]

        public async Task<IActionResult> DeleteTransactionCharge([FromBody] DeleteRequest item)
        {
            try
            {
                foreach (var id in item.ItemIds)
                {
                    await _repo.DeleteTransactionChargeAsync(id);
                }
                return Ok(
                new DeleteRespObjt
                {
                    Deleted = true,
                    Status = new APIResponseStatus { Message = new APIResponseMessage { FriendlyMessage = "Successful" } }
                });
            }
            catch (Exception e)
            {
                return BadRequest(
                   new DeleteRespObjt
                   {
                       Deleted = false,
                       Status = new APIResponseStatus { Message = new APIResponseMessage { FriendlyMessage = e.Message, TechnicalMessage = e.ToString() } }
                   });
            } 
        }

        [HttpGet(ApiRoutes.TransactionCharge.DOWNLOAD_TRANSACTIONCHARGE)]
        public async Task<ActionResult<TransactionChargeRespObj>> GenerateExportTransactionCharge()
        {
            var response = _repo.GenerateExportTransactionCharge();

            return new TransactionChargeRespObj
            {
                export = response,
            };
        }

        [HttpPost(ApiRoutes.TransactionCharge.UPLOAD_TRANSACTIONCHARGE)]
        public async Task<ActionResult<TransactionChargeRegRespObj>> UploadTransactionChargeAsync()
        {
            try
            {
                var postedFile = _httpContextAccessor.HttpContext.Request.Form.Files["Image"];
                var fileName = _httpContextAccessor.HttpContext.Request.Form.Files["Image"].FileName;
                var fileExtention = Path.GetExtension(fileName);
                var image = new byte[postedFile.Length];
                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == "userId").Value;

                var createdBy = _serverRequest.UserDataAsync().Result.UserName;

                var isDone = await _repo.UploadTransactionChargeAsync(image, createdBy);
                return new TransactionChargeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = isDone ? true : false, Message = new APIResponseMessage { FriendlyMessage = isDone ? "successful" : "Unsuccessful" } }
                };
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
                return new TransactionChargeRegRespObj
                {
                    Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage { FriendlyMessage = "Error Occurred", TechnicalMessage = ex?.Message, MessageId = errorCode } }
                };
            }
        }

    }
}
