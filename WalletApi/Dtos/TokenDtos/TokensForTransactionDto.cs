namespace WalletApi.Dtos.TokenDtos
{
    public class TokensForTransactionDto
    {
        public UpdateTokenDto TokenFrom { get; }

        public UpdateTokenDto TokenTo { get; }

        public TokensForTransactionDto (UpdateTokenDto tokenFrom, UpdateTokenDto tokenTo)
        {
            TokenFrom = tokenFrom;
            TokenTo = tokenTo;
        }
    }
}
