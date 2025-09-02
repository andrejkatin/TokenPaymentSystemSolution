using WalletApi.Dtos.TokenDtos;

namespace WalletApi.Dtos.WalletDtos
{
    public class WalletDto
    {
        public Guid Address { get; set; }

        public List<TokenDto> Tokens { get; } = new List<TokenDto>();
    }
}
