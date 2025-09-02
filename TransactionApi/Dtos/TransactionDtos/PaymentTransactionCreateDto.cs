using System.ComponentModel.DataAnnotations;
using TransactionEFDAL.Models.Enums;

namespace TransactionApi.Dtos.TransactionDtos
{
    public class PaymentTransactionCreateDto
    {
        [Required]
        public string From{ get; set; }

        [Required]
        public string To{ get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TokenTypeEnum TokenType { get; set; }

        [Required]
        public TransactionTypeEnum TransactionType { get; set; }
    }
}
