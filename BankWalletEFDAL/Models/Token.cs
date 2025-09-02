using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankWalletEFDAL.Models
{
    public class Token
    {
        [Key]
        public Guid TokenId { get; set; }

        [Required]
        public int TokenIdHash { get; set; }

        [Required]
        public string TokenJson { get; set; }

        [ForeignKey("Wallet")]
        [Required]
        public Guid WalletAddress { get; set; }

        [Required]
        public int WalletAddressHash { get; set; }

        public Wallet Wallet { get; set; }
    }
}
