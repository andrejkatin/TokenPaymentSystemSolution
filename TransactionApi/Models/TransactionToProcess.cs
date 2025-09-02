using TransactionApi.Dtos.TokenDtos;
using TransactionEFDAL.Models;

namespace TransactionApi.Models
{
    public class TransactionToProcess
    {
        public Transaction Transaction { get; set; }

        public TokensForTransactionDto CreatedTokens { get; set; }
    }
}
