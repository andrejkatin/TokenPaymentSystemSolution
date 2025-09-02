using WalletApi.Dtos.Enums;
using WalletApi.Dtos.TokenDtos;

namespace WalletApi.Services
{
    public interface ITokenService
    {
        Task<UpdateTokenDto> GetTokenForTransaction(Guid address, TokenTypeEnum tokenType);
        Task<TokensForTransactionDto> GetTokensForTransaction(Guid fromAddress, Guid toAddress, TokenTypeEnum tokenType);
        Task UpdateTokensByTransaction(UpdateTokenDto fromTokenUpdated, UpdateTokenDto toTokenUpdated);
    }
}
