using TokenApi.Dtos.TokenDtos;

namespace TokenApi.Models.TokenVersions
{
    public interface IToken
    {
        bool IsTokenValid();

        TokenDto ToTokenDto();
    }
}
