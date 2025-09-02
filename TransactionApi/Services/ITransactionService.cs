using TransactionApi.Dtos.TokenDtos;
using TransactionApi.Dtos.TransactionDtos;
using TransactionApi.Models;
using TransactionEFDAL.Models.Enums;

namespace TransactionApi.Services
{
    public interface ITransactionService
    {
        Task<TokenDto> GetTokenForTransaction(TokenTypeEnum tokenType, string address);

        Task<TokensForTransactionDto> GetTokensForTransaction(TokenTypeEnum tokenType, string from, string to);

        Task<TokenDto> CreateNewToken(Guid walletAddress, Guid transactionId, TransactionTypeEnum transactionType, TokenDto currentToken, decimal amount, TokenTypeEnum tokenType);

        Task<TokensForTransactionDto> CreateNewTokens(Guid transactionId, TokensForTransactionDto currentTokens, decimal amount, TokenTypeEnum tokenType);

        TransactionToProcess CreateTransactionToProcess(Guid transactionId, TransactionStatus status, string iniciator, TokensForTransactionDto currentTokens, TokensForTransactionDto newTokens, decimal amount, TokenTypeEnum tokenType);

        void AddTransactionToPool(TransactionToProcess transactionToProcess);
    }
}
