using TokenApi.Dtos.Enums;
using TokenApi.Dtos.TokenDtos;
using TokenApi.Models.TokenVersions.V1;

namespace TokenApi.Models.TokenVersions
{
    public class TokenFactory
    {
        public IToken GetToken(string version, TokenDto token)
        {
            switch (version)
            {
                case "1.0":
                    return new TokenV1(token);
                default:
                    throw new Exception();
            }
        }

        public IToken CreateToken(Guid walletAddress, decimal amount, Guid transactionId, TransactionTypeEnum transactionType, TokenTypeEnum tokenType)
        {
            if (transactionType != TransactionTypeEnum.Deposit)
            {
                throw new Exception("Token creation is posible only for deposit transaction type.");
            }
            return new TokenV1(walletAddress, amount, transactionId, tokenType);
        }

        public IToken UpdateToken(TokenDto tokenDto, TransactionTypeEnum transactionType, decimal amount)
        {
            var token = new TokenV1(tokenDto);
            if (transactionType == TransactionTypeEnum.Deposit)
            {
                token.TokenData.Balance += amount;
            }
            if (transactionType == TransactionTypeEnum.Withdraw)
            {
                token.TokenData.Balance -= amount;
            }
            return token;
        }
    }
}
