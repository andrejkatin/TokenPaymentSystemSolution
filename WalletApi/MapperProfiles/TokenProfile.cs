using AutoMapper;
using WalletApi.Dtos.TokenDtos;
using WalletEFDAL.Models;

namespace WalletApi.MapperProfiles
{
    public class TokenProfile : Profile
    {
        public TokenProfile()
        {
            CreateMap<UpdateTokenDto, Token>().ReverseMap();
        }
    }
}
