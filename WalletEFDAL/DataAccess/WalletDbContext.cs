using Microsoft.EntityFrameworkCore;
using WalletEFDAL.Models;

namespace WalletEFDAL.DataAccess
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

        public virtual DbSet<Wallet> Wallets { get; set; }

        public virtual DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelSetupWallet(modelBuilder);
            ModelSetupToken(modelBuilder);
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
