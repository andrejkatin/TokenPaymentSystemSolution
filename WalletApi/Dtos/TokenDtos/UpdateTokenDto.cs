using System.ComponentModel.DataAnnotations;

namespace WalletApi.Dtos.TokenDtos
{
    public class UpdateTokenDto
    {
        [Required]
        public Guid TokenId { get; set; }

        [Required]
        public int TokenIdHash { get; set; }

        [Required]
        public string TokenData { get; set; }

        [Required]
        public int TokenDataSignature { get; set; }

        [Required]
        public Guid WalletAddress { get; set; }

        [Required]
        public int WalletAddressHash { get; set; }
    }

}
