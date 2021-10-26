using Banking.Contracts.Command;
using Banking.Data;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Banking.Validators.Deposit
{
    public class AddUpdateBankClosureCommandVal : AbstractValidator<AddUpdateBankClosureCommand>
    {
        public AddUpdateBankClosureCommandVal()
        {
            RuleFor(x => x.AccountNumber).NotEmpty().Length(10, 10);
            RuleFor(x => x.ApproverName).NotEmpty().Length(2, 200);
        
        }
    }

    
    public class AddUpdateBankClosureSetupCommandVal : AbstractValidator<AddUpdateBankClosureSetupCommand>
    {
        private readonly DataContext _dataContext;
        public AddUpdateBankClosureSetupCommandVal(DataContext dataContext)
        {
            _dataContext = dataContext;
            RuleFor(e => e.Amount).NotEmpty().WithMessage("Some required");
            RuleFor(e => e.ProductId).NotEmpty().WithMessage("Product required");
            RuleFor(e => e.Charge).NotEmpty().WithMessage("Charge required");
            RuleFor(e => e.Percentage).NotEmpty().WithMessage("Percentage required").MustAsync(ValidPercentageAsync).WithMessage("Invalid percentage value");  
            RuleFor(r => r).MustAsync(NoDuplicateAsync).WithMessage("Duplicate setup detected");
        }

        private async Task<bool> ValidPercentageAsync(double percentage, CancellationToken cancellationToken)
        {
            if (percentage > 100 || percentage < 1)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
        private async Task<bool> NoDuplicateAsync(AddUpdateBankClosureSetupCommand request, CancellationToken cancellationToken)
        {
            if (request.BankClosureSetupId > 0)
            {
                var item = _dataContext.deposit_bankclosuresetup.FirstOrDefault(e => e.Amount == request.Amount && e.ProductId == request.ProductId && e.BankClosureSetupId!= request.BankClosureSetupId && e.Deleted == false);
                if (item != null)
                {
                    return await Task.Run(() => false);
                }
                return await Task.Run(() => true);
            }
            if (_dataContext.deposit_bankclosuresetup.Count(e => e.Amount == request.Amount && e.ProductId == request.ProductId && e.Deleted == false) >= 1)
            {
                return await Task.Run(() => false);
            }
            return await Task.Run(() => true);
        }
    }
}
