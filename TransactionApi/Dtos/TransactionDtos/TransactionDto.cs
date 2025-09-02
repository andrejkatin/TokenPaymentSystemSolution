
using TransactionApi.Dtos.TokenDtos;
using TransactionEFDAL.Models.Enums;

namespace TransactionApi.Dtos.TransactionDtos
{
    public class TransactionDto
    {
        public TokenDto FromOldToken { get; set; }

        public TokenDto FromNewToken { get; set; }

        public TokenDto ToOldToken { get; set; }

        public TokenDto ToNewToken { get; set; }

        public string FeeAmmount { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }
    }
}
