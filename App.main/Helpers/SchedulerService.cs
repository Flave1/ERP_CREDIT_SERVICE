using AutoMapper;
using Banking.AuthHandler.Interface;
using Banking.Contracts.Response.Mail;
using Banking.Data;
using Banking.DomainObjects.Credit;
using Banking.Repository.Implement.Credit;
using Banking.Repository.Interface.Credit;
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
    public class SchedulerService : CronJobService
    {
        private readonly ILogger<SchedulerService> _logger;

        private readonly ILoanRepository _repo;
        private readonly DataContext _dataContext;
        private readonly IIdentityServerRequest _serverRequest;
        private readonly ILoanScheduleRepository _schedule;
        public SchedulerService(IServiceScopeFactory _factory, IIdentityServerRequest serverRequest,
        IScheduleConfig<SchedulerService> config,
        ILogger<SchedulerService> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _repo = _factory.CreateScope().ServiceProvider.GetRequiredService<ILoanRepository>();
            _logger = logger;
            _dataContext = _factory.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
            _serverRequest = serverRequest;
            //_serverRequest = _factory.CreateScope().ServiceProvider.GetRequiredService<IIdentityServerRequest>();
            _schedule = _factory.CreateScope().ServiceProvider.GetRequiredService<ILoanScheduleRepository>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler Service starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var loanList = await (from a in _dataContext.credit_loan
                                      where a.Deleted == false && a.IsUploaded == true
                                      select a).ToListAsync();
                var staffEmail = string.Empty;
                if (loanList.Count() > 0)
                {
                    foreach (var item in loanList)
                    {
                        staffEmail = item.staffEmail;
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

                            item.IsUploaded = false;
                        }
                        catch (Exception ex)
                        {
                            var customer = await _dataContext.credit_loancustomer.FindAsync(item.CustomerId);
                            var product = await _dataContext.credit_product.FindAsync(item.ProductId);
                            var msg = $"Hi, <br> The loan schedule generation was not successful for the uploaded loans due to the following reasons: <br/>" +
                                $"{ex.Message}. <br/> This happened on the loan with product name of {product.ProductName} and customer with the email of {customer.Email}";
                            await _serverRequest.SendMail(new MailObj
                            {
                                fromAddresses = new List<FromAddress> { },
                                toAddresses = new List<ToAddress>
                        {
                            new ToAddress{ address = item.staffEmail, name = ""}
                        },
                                subject = "Error Occured while on Loan Schedule Generation",
                                content = msg,
                                sendIt = false,
                                saveIt = true,
                                module = 2,
                                userIds = ""
                            });
                        }
                    }
                    //_dataContext.credit_loan.AttachRange(loanList);
                    //_dataContext.credit_loan.UpdateRange(loanList);
                    //await _dataContext.SaveChangesAsync();

                    await _serverRequest.SendMail(new MailObj
                    {
                        fromAddresses = new List<FromAddress> { },
                        toAddresses = new List<ToAddress>
                        {
                            new ToAddress{ address = staffEmail, name = ""}
                        },
                        subject = "Successful Loan Schedule Generation",
                        content = $"Hi, <br> The loan schedule generation was successful for the uploaded loans",
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

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Scheduler Service is working.");
            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler Service is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
