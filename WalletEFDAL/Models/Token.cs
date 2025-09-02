using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletEFDAL.Models
{
    public class Token
    {
        [Key]
        public Guid TokenId { get; set; }

        [Required]
        public int TokenIdHash { get; set; }

        [Required]
        public string TokenData {  get; set; }

        [Required]
        public int TokenDataSignature { get; set; }

        [ForeignKey("Wallet")]
        [Required]
        public Guid WalletAddress { get; set; }

        [Required]
        public int WalletAddressHash { get; set; }

        public Wallet Wallet { get; set; }
    }
}
