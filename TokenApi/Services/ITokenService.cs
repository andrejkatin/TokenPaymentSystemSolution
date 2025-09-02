using TokenApi.Dtos.Enums;
using TokenApi.Dtos.TokenDtos;

namespace TokenApi.Services
{
    public interface ITokenService
    {
        TokenDto CreateNewToken(Guid walletAddress, Guid TransactionId, TransactionTypeEnum transactionType, TokenTypeEnum tokenType, decimal amount, TokenDto currentToken);
        bool ValidateToken(TokenDto currentToken);
    }
}
