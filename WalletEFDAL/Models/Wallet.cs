using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletEFDAL.Models
{
    public class Wallet
    {
        [Key]
        public Guid Address { get; set; }

        public ICollection<Token> Tokens { get; } = new List<Token>();
    }
}
