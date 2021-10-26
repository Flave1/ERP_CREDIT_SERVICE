using Banking.AuthHandler.Interface;
using Banking.Contracts.Response.IdentityServer;
using Banking.Data;
using Banking.DomainObjects.Auth;
using Banking.Requests;
using GOSLibraries.Enums;
using GOSLibraries.GOS_API_Response;
using GOSLibraries.GOS_Error_logger.Service;
using GOSLibraries.GOS_Financial_Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Wangkanai.Detection.Services;

namespace Banking.Handlers.Auths
{

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IIdentityServerRequest _service; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDetectionService _detectionService;
        private readonly DataContext _securityContext;
        private readonly ILoggerService _logger;
        private readonly IIdentityService _identityService;
        public LoginCommandHandler(
            IIdentityServerRequest identityRepoService,
            UserManager<ApplicationUser> userManager, 
            DataContext dataContext,
            IIdentityService identityService,
            IDetectionService detectionService,
            ILoggerService loggerService)
        {
            _userManager = userManager; 
            _service = identityRepoService; 
            _securityContext = dataContext;
            _logger = loggerService;
            _detectionService = detectionService;
            _identityService = identityService;
        }

        public async Task<AuthResponse> OTPOptionsAsync(ApplicationUser user)
        {
            try
            { 
                var response = new AuthResponse { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
                var settings = await _service.GetSettingsAsync()??new SecurityResp { authSettups = new List<Security>()};
                if(settings.authSettups.Count() > 0)
                {
                    var multiplefFA = settings.authSettups.Where(a => a.Module == (int)Modules.CREDIT); 
                    if (multiplefFA.Count() > 0)
                    {
                        if (_detectionService.Device.Type.ToString().ToLower() == Device.Desktop.ToString().ToLower())
                        {
                            if (multiplefFA.FirstOrDefault(a => a.Media == (int)Media.EMAIL).ActiveOnWebApp)
                            {
                                await _identityService.SendOTPToEmailAsync(user);
                                response.Status.Message.FriendlyMessage = "OTP Verification Code sent to your email";
                                return response;
                            }
                            if (multiplefFA.FirstOrDefault(a => a.Media == (int)Media.SMS) != null && multiplefFA.FirstOrDefault(a => a.Media == (int)Media.SMS).ActiveOnWebApp)
                            {
                                response.Status.Message.FriendlyMessage = "OTP Verification Code sent to your number";
                                return response;
                            }
                        }
                        if (_detectionService.Device.Type.ToString().ToLower() == Device.Mobile.ToString().ToLower())
                        {
                            if (multiplefFA.FirstOrDefault(a => a.Media == (int)Media.EMAIL).ActiveOnMobileApp)
                            {
                                await _identityService.SendOTPToEmailAsync(user);
                                response.Status.Message.FriendlyMessage = "OTP Verification Code sent to your email";
                                return response;
                            }
                            if (multiplefFA.FirstOrDefault(a => a.Media == (int)Media.SMS).ActiveOnMobileApp)
                            {
                                response.Status.Message.FriendlyMessage = "OTP Verification Code sent to your number";
                                return response;
                            }
                        }
                    }
                }
                
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
 

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = new AuthResponse { Status = new APIResponseStatus { IsSuccessful = false, Message = new APIResponseMessage() } };
            try
            {

                var user_acccount = await _userManager.FindByEmailAsync(request.UserName) ?? null;
                if (!await _identityService.ReturnStatusAsync(user_acccount))
                { 
                    return response;
                }
                if (!await IsPasswordCharactersValid(request.Password))
                { 
                    response.Status.Message.FriendlyMessage = "Invalid Password";
                    return response;
                }
                if (!await UserExist(request, user_acccount))
                { 
                    response.Status.Message.FriendlyMessage = "User does not exist";
                    return response;
                }
                if (!await IsValidPassword(request, user_acccount))
                { 
                    response.Status.Message.FriendlyMessage = "User/Password Combination is wrong";
                    return response;
                }
                  
                var otp = await OTPOptionsAsync(user_acccount);
                if (otp.Status.IsSuccessful)
                {
                    response.Status.IsSuccessful = true;
                    otp.Status.Message.MessageId = user_acccount.Email;
                    return otp;
                }

                var result = await _identityService.CustomerLoginAsync(user_acccount);

                response.Status.IsSuccessful = true;
                response.Token = result.Token;
                response.RefreshToken = result.RefreshToken;
                return response;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage = ex?.Message ?? ex?.InnerException?.Message;
                response.Status.Message.TechnicalMessage = ex.ToString();
                return response;
            }
        }

        private async Task<bool> IsValidPassword(LoginCommand request, ApplicationUser user)
        { 
            var isValidPass = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPass)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
        private async Task<bool> UserExist(LoginCommand request, ApplicationUser user)
        { 
            if (user == null)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
        private async Task<bool> IsPasswordCharactersValid(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return await Task.Run(() => hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password));
        }

    }

}
