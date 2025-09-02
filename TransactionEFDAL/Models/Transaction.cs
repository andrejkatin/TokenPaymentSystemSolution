using System.ComponentModel.DataAnnotations;
using TransactionEFDAL.Models.Enums;

namespace TransactionEFDAL.Models
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        [Required]
        public TransactionTypeEnum TransactionType { get; set; }

        public string TokenFromOld { get; set; }

        public string TokenToOld { get; set; }

        [Required]
        public TokenTypeEnum TokenType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal FeeFrom { get; set; }

        [Required]
        public decimal FeeTo { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public TransactionStatus TransactionStatus { get; set; }

        [Required]
        public string TransactionSignature { get; set; }

        [Required]
        public string TransactionIniciator {  get; set; }

        public string ErrorMsg {  get; set; }

        public string Description { get; set; }

        public string EtheriumProof {  get; set; }
    }
}
