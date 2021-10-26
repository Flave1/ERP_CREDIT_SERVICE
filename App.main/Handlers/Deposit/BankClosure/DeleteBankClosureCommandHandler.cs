using Banking.Contracts.Command;
using Banking.Data;
using GOSLibraries.GOS_API_Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks; 

namespace Banking.Handlers.Deposit.AccountSetup
{ 
    public class DeleteBankClosureCommandHandler : IRequestHandler<DeleteBankClosureCommand, Banking.Contracts.Response.Deposit.DeleteRespObj>
    {
		private readonly DataContext _dataContext;
		public DeleteBankClosureCommandHandler(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
        public async Task<Banking.Contracts.Response.Deposit.DeleteRespObj> Handle(DeleteBankClosureCommand request, CancellationToken cancellationToken)
        {
			var resp = new Contracts.Response.Deposit.DeleteRespObj { Status = new APIResponseStatus { IsSuccessful = true, Message = new APIResponseMessage() } };
			try
			{
				if(request.BankClosureIds.Count() > 0)
				{
					foreach(var id in request.BankClosureIds)
					{
						var item = await _dataContext.deposit_bankclosure.FindAsync(id);
						if(item != null)
						{
							_dataContext.deposit_bankclosure.Remove(item);
							_dataContext.SaveChanges();
						}
					}
					resp.Status.Message.FriendlyMessage = "Successful";
					return resp;
				}
				else
				{
					resp.Deleted = false;
					resp.Status.Message.FriendlyMessage = "Please Select Item to delete" ;
					return resp;
				}
			}
			catch (Exception e)
			{ 
				throw e;
			}
        }
    }

}
