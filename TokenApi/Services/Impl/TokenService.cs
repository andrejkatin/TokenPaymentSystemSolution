using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TokenApi.Dtos.Enums;
using TokenApi.Dtos.TokenDtos;
using TokenApi.Models.TokenVersions;

namespace TokenApi.Services.Impl
{
    public class TokenService : ITokenService
    {
        public TokenService() { }

        public TokenDto CreateNewToken(Guid walletAddress, Guid TransactionId, TransactionTypeEnum transactionType, TokenTypeEnum tokenType, decimal amount, TokenDto currentToken)
        {
            var tokenFactory = new TokenFactory();
            if (currentToken.TokenId == Guid.Empty)
            {
                return tokenFactory.CreateToken(walletAddress, amount, TransactionId, transactionType, tokenType).ToTokenDto();
            }

            var isValid = ValidateToken(currentToken);
            if (!isValid)
            {
                throw new Exception("Invalid token with Id: " + currentToken.TokenId);
            }

            return tokenFactory.UpdateToken(currentToken, transactionType, amount).ToTokenDto();
        }

        public bool ValidateToken(TokenDto currentToken)
        {
            var tokenVersion = JsonConvert.DeserializeObject<JObject>(currentToken.TokenData)["Version"].ToString();
            var tokenFactory = new TokenFactory();
            IToken token = tokenFactory.GetToken(tokenVersion, currentToken);
            return token.IsTokenValid();
        }
    }
}
