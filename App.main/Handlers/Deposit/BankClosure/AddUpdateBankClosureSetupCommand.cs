using Banking.Contracts.Command;
using Banking.Contracts.Response.Deposit;
using Banking.Data;
using GODP.Entities.Models;
using GOSLibraries.GOS_API_Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Banking.Handlers.Deposit.BankClosure
{
    public class AddUpdateBankClosureSetupCommandHandler : IRequestHandler<AddUpdateBankClosureSetupCommand, Deposit_bankClosureSetupRegRespObj>
    {
		private readonly DataContext _dataContext;
		public AddUpdateBankClosureSetupCommandHandler(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
        public async Task<Deposit_bankClosureSetupRegRespObj> Handle(AddUpdateBankClosureSetupCommand request, CancellationToken cancellationToken)
        {
            var response = new Deposit_bankClosureSetupRegRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
            try
            {   var Trate = _dataContext.deposit_bankclosuresetup.Find(request.BankClosureSetupId);
                if(Trate == null)
                    Trate = new deposit_bankclosuresetup();

                Trate.BankClosureSetupId = request.BankClosureSetupId;
                Trate.Structure = request.Structure;
                Trate.ProductId = request.ProductId;
                Trate.ClosureChargeApplicable = request.ClosureChargeApplicable;
                Trate.Charge = request.Charge;
                Trate.ChargeType = request.ChargeType;
                Trate.PresetChart = request.PresetChart;
                Trate.SettlementBalance = request.SettlementBalance;
                Trate.Amount = request.Amount;
                Trate.PresetChart = request.PresetChart;
                Trate.Percentage = request.Percentage;
                if(Trate.BankClosureSetupId > 0)
                {
                    var item = _dataContext.deposit_bankclosuresetup.Find(Trate.BankClosureSetupId);
                    if (item != null) 
                        _dataContext.Entry(item).CurrentValues.SetValues(Trate); 
                }
                else
                    _dataContext.deposit_bankclosuresetup.Add(Trate);
                _dataContext.SaveChanges();

                response.BankClosureSetupId = Trate.BankClosureSetupId;
                response.Status.Message.FriendlyMessage = "Successful";
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
