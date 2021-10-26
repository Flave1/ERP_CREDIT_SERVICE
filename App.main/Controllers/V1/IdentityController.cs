using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc; 
using Banking.AuthHandler.Interface;
using GOSLibraries.GOS_API_Response;
using Banking.Contracts.V1;
using GOSLibraries.GOS_Financial_Identity;
using Banking.Requests;
using Banking.Contracts.Response;
using AuthSuccessResponse = Banking.Contracts.Response.AuthSuccessResponse;
using System;
using Banking.Repository.Interface.Credit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GOSLibraries.GOS_Error_logger.Service;
using System.Linq;
using Banking.Contracts.Response.Finance;
using GOSLibraries.Enums;
using MediatR;
using Banking.Handlers.Auths;
using APIGateway.AuthGrid.Recovery;
using Banking.Contracts.Response.Auth;

namespace Libraryhub.Controllers.V1
{

    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IIdentityServerRequest _identityServer;
        private readonly ILoanCustomerRepository _customerRepo;
        private readonly ILoggerService _logger;
        private readonly IMediator _mediator;

        public IdentityController(IIdentityService identityService,
            IIdentityServerRequest identityServer, IMediator mediator,
            ILoanCustomerRepository customerRepo, ILoggerService logger )
        {
            _identityServer = identityServer;
            _identityService = identityService;
            _customerRepo = customerRepo;
            _logger = logger;
            _mediator = mediator;
        }

        
        [HttpPost(ApiRoutes.Identity.LOGIN)]
        public async Task<IActionResult> Login([FromBody] UserLoginReqObj request)
        {
            var authResponse = await _identityServer.LoginAsync(request.UserName, request.Password);
            if (authResponse.Token == null)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Status = new APIResponseStatus
                    {
                        IsSuccessful = authResponse.Status.IsSuccessful,
                        Message = new APIResponseMessage
                        {
                            FriendlyMessage = authResponse?.Status?.Message?.FriendlyMessage,
                            TechnicalMessage = authResponse?.Status?.Message?.TechnicalMessage
                        }
                    }
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }


        #region LOAN_CUSTOMER_USER_MANAGER

        [HttpPost(ApiRoutes.Identity.CUSTOMER_REGISTER)]
        public async Task<IActionResult> CUSTOMER_REGISTER([FromBody] RegistrationCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
         
        [HttpPost(ApiRoutes.Identity.CUSTOMER_LOGIN)]
        public async Task<IActionResult> CUSTOMER_LOGIN([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            var securityResp = await _identityServer.CheckForFailedTrailsAsync(response.Status.IsSuccessful, (int)Modules.CREDIT, command.UserName);
            if (securityResp.Status.IsSuccessful)
            {
                if (response.Status.IsSuccessful)
                {
                    return Ok(response);
                }
                await _identityService.UnlockUserAsync(command.UserName);
                return BadRequest(response);
            }
            await _identityService.PerformLockFunction(command.UserName, securityResp.UnLockAt, securityResp.IsSecurityQuestion);
            return BadRequest(securityResp);
        }

        [HttpPost(ApiRoutes.Identity.CUSTOMER_OTP_LOGIN)]
        public async Task<IActionResult> CUSTOMER_OTP_LOGIN([FromBody] OTPLoginCommand request)
        {
            var resp = await _mediator.Send(request);
            if (resp.Status.IsSuccessful)
                return Ok(resp);
            return BadRequest(resp);
        }
        [HttpPost(ApiRoutes.Identity.ANSWER)]
        public async Task<IActionResult> ANSWER([FromBody] AnswerQuestionsCommand request)
        {
            var resp = await _mediator.Send(request);
            if (resp.Status.IsSuccessful)
                return Ok(resp);
            return BadRequest(resp);
        }

        [HttpGet(ApiRoutes.Identity.GET_ANSWER)]
        public async Task<IActionResult> GET_ANSWER([FromQuery] GetQuestionQuery request)
        {
            var resp = await _mediator.Send(request);
            if (resp.Status.IsSuccessful)
                return Ok(resp);
            return BadRequest(resp);
        }



        [HttpPost(ApiRoutes.Identity.RECOVER_PASSWORD_BY_EMAIL)]
        public async Task<IActionResult> RECOVER_PASSWORD_BY_EMAIL([FromBody] RecoverAccountByEmailCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost(ApiRoutes.Identity.NEW_PASS)]
        public async Task<IActionResult> NEW_PASS([FromBody] ChangePasswordCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Status.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

     

        public string token { get; set; }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Identity.FETCH_CUSTOMER_USERDETAILS)]
        public async Task<ActionResult<UserDataResponseObj>> GetUserProfile()
        {
            string userId = HttpContext.User?.FindFirst(c => c.Type == "userId").Value;

            var profile = await _identityService.CustomerFetchLoggedInUserDetailsAsync(userId);

            if (!profile.Status.IsSuccessful)
            {
                return BadRequest(profile.Status);
            }
            var customerDetail = await _customerRepo.GetCustomerByEmailAsync(profile.Email);
            if (customerDetail != null)
            {
                profile.CustomerId = customerDetail.CustomerId;
                profile.ProfileStatus = customerDetail.ProfileStatus;
                profile.AccountNumber = customerDetail.CASAAccountNumber;
            }
            return Ok(profile);
        }
        #endregion

    }
}