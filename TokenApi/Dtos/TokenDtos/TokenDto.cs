using System.ComponentModel.DataAnnotations;

namespace TokenApi.Dtos.TokenDtos
{
    public class TokenDto
    {
        public Guid TokenId { get; set; }

        public int TokenIdHash { get; set; }

        public string TokenData { get; set; }

        public int TokenDataSignature { get; set; }

        public Guid WalletAddress { get; set; }

        public int WalletAddressHash { get; set; }
    }
}
