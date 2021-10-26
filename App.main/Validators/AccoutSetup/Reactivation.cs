using Banking.Data;
using Banking.Handlers.Deposit.BankClosure;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Banking.Validators.AccoutSetup
{
    public class AddUpdateReactivationAccountSetupCommandVal : AbstractValidator<AddUpdateReactivationAccountSetupCommand>
    {
        private readonly DataContext _dataContext;
        public AddUpdateReactivationAccountSetupCommandVal(DataContext data)
        {
            _dataContext = data;
            RuleFor(r => r.Amount).NotEmpty().WithMessage("Amount Required");
            RuleFor(r => r.Charge).NotEmpty(); 
            RuleFor(r => r.ChargeType).NotEmpty().WithMessage("Charge Type Required");
            RuleFor(r => r.Product).NotEmpty().WithMessage("Product Required");  
            
        }

        //private async Task<bool> NoDuplicateAsync(AddUpdateReactivationAccountSetupCommand request, CancellationToken cancellationToken)
        //{

        //    if (_dataContext.deposit_accountsetup.Count(w => w.AccountName.ToLower().Trim() == request.AccountName.ToLower().Trim() && request.AccountTypeId != w.AccountTypeId) == 1)
        //    {
        //        return await Task.Run(() => false);
        //    }
        //    return await Task.Run(() => true);
        //}
    }
}
