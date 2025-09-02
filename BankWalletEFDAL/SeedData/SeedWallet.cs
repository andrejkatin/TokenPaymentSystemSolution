using BankWalletEFDAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankWalletEFDAL.SeedData
{
    public static class SeedWallet
    {
        private static Guid BankId = Guid.Parse("8276b457-a1da-46c3-9ca3-ce1f8fb65039");

        public static void SeedData(ModelBuilder modelBuilder)
        {
            Wallet bankWallet = new Wallet()
            {
                Address = BankId
            };

            modelBuilder.Entity<Wallet>().HasData(bankWallet);
        }
    }
}
