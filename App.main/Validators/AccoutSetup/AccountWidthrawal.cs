using Banking.Contracts.Response.Deposit;
using Banking.Data;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Banking.Validators.AccoutSetup
{
    public class AddUpdateWithdrawalSetupObjVal : AbstractValidator<AddUpdateWithdrawalSetupObj>
    {
        private readonly DataContext _dataContext;
        public AddUpdateWithdrawalSetupObjVal(DataContext dataContext)
        {
            _dataContext = dataContext; 
            RuleFor(e => e.Product).NotEmpty();
            RuleFor(e => e.ChargeType).NotEmpty();
            RuleFor(e => e.Charge).NotEmpty();
            RuleFor(e => e.Amount).NotEmpty();
            RuleFor(e => e.AccountType).NotEmpty();
            RuleFor(e => e).MustAsync(NoDuplicateAsync).WithMessage("Duplicate setup detected");
        }

        private async Task<bool> NoDuplicateAsync(AddUpdateWithdrawalSetupObj request, CancellationToken cancellationToken)
        {
            if (request.WithdrawalSetupId > 0)
            {
                var item = _dataContext.deposit_withdrawalsetup.FirstOrDefault(e => e.Charge == request.Charge &&  e.ChargeType == request.ChargeType && e.WithdrawalSetupId != request.WithdrawalSetupId && e.Deleted == false);
                if (item != null)
                {
                    return await Task.Run(() => false);
                }
                return await Task.Run(() => true);
            }
            if (_dataContext.deposit_withdrawalsetup.Count(e => e.Charge == request.Charge && e.ChargeType == request.ChargeType && e.Deleted == false) >= 1)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
    }
}
