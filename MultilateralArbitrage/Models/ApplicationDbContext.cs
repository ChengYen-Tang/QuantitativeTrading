using Microsoft.EntityFrameworkCore;
using System;

namespace MultilateralArbitrage.Models
{
    internal class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=MultilateralArbitrage;User ID=sa;Password=P@ssw0rd;", opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(30).TotalSeconds));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetsRecord>(eb => eb.HasNoKey());
        }

        public virtual DbSet<AssetsRecord> AssetsRecords { get; set; }
    }

    [Keyless]
    internal class AssetsRecord
    {
        public string MarketMix { get; set; }
        public DateTime Date { get; set; }
        public float Assets { get; set; }
    }
}
