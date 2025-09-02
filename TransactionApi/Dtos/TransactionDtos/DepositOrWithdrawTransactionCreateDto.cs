using System.ComponentModel.DataAnnotations;
using TransactionEFDAL.Models.Enums;

namespace TransactionApi.Dtos.TransactionDtos
{
    public class DepositOrWithdrawTransactionCreateDto
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TokenTypeEnum TokenType { get; set; }

        [Required]
        public TransactionTypeEnum TransactionType { get; set; }
    }
}
