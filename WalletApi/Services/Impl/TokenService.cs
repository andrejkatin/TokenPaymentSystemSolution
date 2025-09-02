using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WalletApi.Dtos.Enums;
using WalletApi.Dtos.TokenDtos;
using WalletEFDAL.DataAccess;
using WalletEFDAL.Models;

namespace WalletApi.Services.Impl
{
    public class TokenService : ITokenService
    {
        private readonly WalletDbContext Context;
        private readonly IMapper Mapper;

        public TokenService(WalletDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public async Task<UpdateTokenDto> GetTokenForTransaction(Guid address, TokenTypeEnum tokenType)
        {
            var token = await Context.Tokens
                .FirstOrDefaultAsync(t => t.WalletAddress == address && EF.Functions.Like(t.TokenData, "%\"TokenTypeId\":" + 0 + "%"));

            return token is null ? new UpdateTokenDto() { WalletAddress = address } : Mapper.Map<UpdateTokenDto>(token);
        }

        public async Task<TokensForTransactionDto> GetTokensForTransaction(Guid fromAddress, Guid toAddress, TokenTypeEnum tokenType)
        {
            var fromToken = await Context.Tokens
                .FirstOrDefaultAsync(t => t.WalletAddress == fromAddress && t.TokenData.Contains("'%\"tokenType\":\""+ tokenType +"\"%'"));

            var toToken = await Context.Tokens
                .FirstOrDefaultAsync(t => t.WalletAddress == toAddress && t.TokenData.Contains("%\"tokenType\":\"" + tokenType + "\"%"));

            return new TokensForTransactionDto(
                fromToken is null ? new UpdateTokenDto() { WalletAddress = fromAddress } : Mapper.Map<UpdateTokenDto>(fromToken),
                toToken is null ? new UpdateTokenDto() { WalletAddress = toAddress } : Mapper.Map<UpdateTokenDto>(toToken));    
        }

        public async Task UpdateTokensByTransaction(UpdateTokenDto fromTokenUpdated, UpdateTokenDto toTokenUpdated)
        {
            if (fromTokenUpdated is not null)
            {
                var fromToken = await Context.Tokens.FirstOrDefaultAsync(t => t.TokenId == fromTokenUpdated.TokenId);
                if (fromToken is null)
                {
                    Context.Add(Mapper.Map<Token>(fromTokenUpdated));
                }
                else
                {
                    UpdateToken(fromToken, fromTokenUpdated);
                }
            }

            if (toTokenUpdated is not null)
            {
                var toToken = await Context.Tokens.FirstOrDefaultAsync(t => t.TokenId == toTokenUpdated.TokenId);
                if (toToken is null)
                {
                    Context.Add(Mapper.Map<Token>(toTokenUpdated));
                }
                else
                {
                    UpdateToken(toToken, toTokenUpdated);
                }
            }

            await Context.SaveChangesAsync();
        }

        private void UpdateToken(Token token, UpdateTokenDto tokenUpdated)
        {
            token.TokenData = tokenUpdated.TokenData;
            token.TokenDataSignature = tokenUpdated.TokenDataSignature;
        }
    }
}
