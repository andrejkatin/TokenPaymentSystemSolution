using AutoMapper;
using WalletApi.Dtos.WalletDtos;
using WalletEFDAL.Models;

namespace WalletApi.MapperProfiles
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<Wallet, WalletDto>();
        }
    }
}
