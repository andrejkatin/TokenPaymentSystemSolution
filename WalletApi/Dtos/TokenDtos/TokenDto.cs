using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WalletEFDAL.Models;
using WalletApi.Dtos.Enums;

namespace WalletApi.Dtos.TokenDtos
{
    public class TokenDto
    {
        public Guid TokenId { get; set; }

        public TokenTypeEnum TokenType { get; set; }

        public decimal Balance { get; set; }
    }
}

