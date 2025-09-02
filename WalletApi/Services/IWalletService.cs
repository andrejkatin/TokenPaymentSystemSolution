using WalletApi.Dtos.WalletDtos;

namespace WalletApi.Services
{
    public interface IWalletService
    {
        Task<WalletDto> GetWallet(string sub);

        Task<(Guid walletAddressFrom, Guid WalletAddressTo)> GetWalletsForTransaction(string subFrom, string subTo);

        Task<WalletDto> CreateWallet(string sub);
    }
}
