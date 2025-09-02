
using BankWalletEFDAL.DataAccess;
using BankWalletEFDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BankWalletApi.Services.Impl
{
    public class BankWalletService : IBankWalletService
    {
        private readonly BankWalletDbContext Context;
        public BankWalletService(BankWalletDbContext context)
        {
            Context = context;
        }
        public async Task<Wallet> GetBankWallet()
        {
            var bankWallet = await Context.Wallets
                .Include(w => w.Tokens)
                .FirstOrDefaultAsync();

            if (bankWallet is null)
            {
                
            }

            return bankWallet;
        }
    }
}
