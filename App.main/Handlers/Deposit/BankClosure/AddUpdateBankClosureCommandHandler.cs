using AutoMapper;
using Banking.Contracts.Command;
using Banking.Contracts.Response.Deposit;
using Banking.Data;
using Banking.DomainObjects;
using GOSLibraries.GOS_API_Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Banking.Handlers.Deposit.BankClosure
{
    public class AddUpdateBankClosureCommandHandler : IRequestHandler<AddUpdateBankClosureCommand, Deposit_bankClosureRegRespObj>
    {
        private readonly DataContext _context;
        private readonly IMapper mapper;
        public AddUpdateBankClosureCommandHandler(DataContext context)
        {
            _context = context;
        }
        public async Task<Deposit_bankClosureRegRespObj> Handle(AddUpdateBankClosureCommand request, CancellationToken cancellationToken)
        {
            var response = new Deposit_bankClosureRegRespObj {  Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
            try
            {
                var Trate = _context.deposit_bankclosure.Find(request.BankClosureId);
                if (Trate == null)
                    Trate = new deposit_bankclosure();

                Trate.Structure = request.Structure;
                Trate.SubStructure = request.SubStructure;
                Trate.AccountName = request.AccountName;
                Trate.AccountNumber = request.AccountNumber;
                Trate.Status = request.Status;
                Trate.AccountBalance = request.AccountBalance;
                Trate.Currency = request.Currency;
                Trate.ClosingDate = request.ClosingDate;
                Trate.Reason = request.Reason;
                Trate.Charges = request.Charges;
                Trate.FinalSettlement = request.FinalSettlement;
                Trate.Beneficiary = request.Beneficiary;
                Trate.ModeOfSettlement = request.ModeOfSettlement;
                Trate.TransferAccount = request.TransferAccount;
                Trate.ApproverName = request.ApproverName;
                Trate.ApproverComment = request.ApproverComment;

                if (Trate.BankClosureId > 0)
                {
                    _context.Entry(Trate).CurrentValues.SetValues(Trate);
                }
                else
                    _context.deposit_bankclosure.Add(Trate);
                await _context.SaveChangesAsync();
                response.Status.Message.FriendlyMessage = "Successful";
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
