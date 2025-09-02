using BankWalletEFDAL.Models;

namespace BankWalletApi.Services
{
    public interface IBankWalletService
    {
        Task<Wallet> GetBankWallet();
    }
}
