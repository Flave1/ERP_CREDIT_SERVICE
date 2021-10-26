using AutoMapper;
using Banking.AuthHandler.Interface;
using Banking.Contracts.Response.Mail;
using Banking.Data;
using Banking.DomainObjects.Credit;
using Banking.Repository.Implement.Credit;
using Banking.Repository.Interface.Credit;
using Banking.Repository.Interface.InvestorFund;
using Banking.Requests;
using GOSLibraries.GOS_Error_logger.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Banking.Helpers.ScheduleConfiguration;

namespace Banking.Helpers
{
    public class SchedulerService2 : CronJobService
    {
        private readonly ILogger<SchedulerService2> _logger;

        private readonly ILoanRepository _repo;
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly IInvestorFundService _invest;
        public SchedulerService2(IServiceScopeFactory _factory, IIdentityServerRequest serverRequest,
        IScheduleConfig<SchedulerService2> config,
        ILogger<SchedulerService2> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _repo = _factory.CreateScope().ServiceProvider.GetRequiredService<ILoanRepository>();
            _logger = logger;
            _dataContext = _factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            _serverRequest = serverRequest;
            //_serverRequest = _factory.CreateScope().ServiceProvider.GetRequiredService<IIdentityServerRequest>();
            _invest = _factory.CreateScope().ServiceProvider.GetRequiredService<IInvestorFundService>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler Service2 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var investmentList = await (from a in _dataContext.inf_investorfund
                                      where a.Deleted == false && a.IsUploaded == true
                                      select a).ToListAsync();
                var staffEmail = string.Empty;
                if (investmentList.Count() > 0)
                {
                    foreach (var item in investmentList)
                    {
                        staffEmail = item.StaffEmail;
                        try
                        {
                            //Generate Schedule
                            _invest.GenerateInvestmentDailyScheduleService(item.InvestorFundId);
                        }
                        catch (Exception ex)
                        {
                            var customer = await _dataContext.credit_loancustomer.FindAsync(item.InvestorFundCustomerId);
                            var product = await _dataContext.inf_product.FindAsync(item.ProductId);
                            var msg = $"Hi, <br> The Investment schedule generation was not successful for the uploaded loans due to the following reasons: <br/>" +
                                $"{ex.Message}. <br/> This happened on the Investment with product name of {product.ProductName} and customer with the email of {customer.Email}";
                            await _serverRequest.SendMail(new MailObj
                            {
                                fromAddresses = new List<FromAddress> { },
                                toAddresses = new List<ToAddress>
                        {
                            new ToAddress{ address = item.StaffEmail, name = ""}
                        },
                                subject = "Error Occured while on Investment Schedule Generation",
                                content = msg,
                                sendIt = false,
                                saveIt = true,
                                module = 2,
                                userIds = ""
                            });
                        }
                    }

                    await _serverRequest.SendMail(new MailObj
                    {
                        fromAddresses = new List<FromAddress> { },
                        toAddresses = new List<ToAddress>
                        {
                            new ToAddress{ address = staffEmail, name = ""}
                        },
                        subject = "Successful Investment Schedule Generation",
                        content = $"Hi, <br> The Investment schedule generation was successful for the uploaded investments",
                        sendIt = false,
                        saveIt = true,
                        module = 2,
                        userIds = ""
                    });
                }
            }
            catch (Exception ex)
            {
                var errorCode = ErrorID.Generate(5);
            }

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Scheduler Service2 is working.");
            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler Service2 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
