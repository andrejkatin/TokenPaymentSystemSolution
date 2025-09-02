using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankWalletEFDAL.Models
{
    public class Wallet
    {
        [Key]
        public Guid Address { get; set; }

        public ICollection<Token> Tokens { get; } = new List<Token>();
    }
}
