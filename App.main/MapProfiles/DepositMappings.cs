using AutoMapper;
using Banking.Contracts.Response.Deposit;
using GODP.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.MapProfiles
{
    public class DepositMappings : Profile
    {
        public DepositMappings()
        {
            CreateMap<deposit_accountsetup, DepositAccountObj>();
            CreateMap<deposit_withdrawalsetup, WithdrawalSetupObj>();
            CreateMap<deposit_transfersetup, TransferSetupObj>();
        }
    }
}
