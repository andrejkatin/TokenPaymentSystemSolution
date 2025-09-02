using Microsoft.EntityFrameworkCore;
using TransactionEFDAL.Models;

namespace TransactionEFDAL.DataAccess
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelSetupTransaction(modelBuilder);
        }

        private void ModelSetupTransaction(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId);
        }
    }
}
