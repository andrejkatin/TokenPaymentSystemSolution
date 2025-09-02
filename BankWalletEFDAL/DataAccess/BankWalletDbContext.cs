using BankWalletEFDAL.Models;
using BankWalletEFDAL.SeedData;
using Microsoft.EntityFrameworkCore;

namespace BankWalletEFDAL.DataAccess
{
    public class BankWalletDbContext : DbContext
    {
        public BankWalletDbContext(DbContextOptions<BankWalletDbContext> options) : base(options) { }

        public virtual DbSet<Wallet> Wallets { get; set; }

        public virtual DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelSetupWallet(modelBuilder);
            ModelSetupToken(modelBuilder);
            SeedWallet.SeedData(modelBuilder);
        }

        private void ModelSetupWallet(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wallet>()
                .HasKey(w => w.Address);
        }

        private void ModelSetupToken(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Token>()
                .HasKey(t => t.TokenId);
        }
    }
}
