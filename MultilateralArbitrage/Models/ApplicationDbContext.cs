using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MultilateralArbitrage.Models
{
    internal class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=MultilateralArbitrage;User ID=sa;Password=P@ssw0rd;", opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(30).TotalSeconds));
        }

        public virtual DbSet<CollisionAssetsRecord> CollisionAssetsRecords { get; set; }
        public virtual DbSet<CollisionAndLastStepPaddingAssetsRecord> CollisionAndLastStepPaddingAssetsRecords { get; set; }
    }

    internal class AssetsRecord
    {
        public AssetsRecord()
            => (Id, Date) = (Guid.NewGuid(), DateTime.Now);

        [Key]
        public Guid Id { get; set; }
        public string MarketMix { get; set; }
        public DateTime Date { get; set; }
        public float Assets { get; set; }
    }

    internal class CollisionAssetsRecord : AssetsRecord
    {
        public CollisionAssetsRecord() : base() { }
    }

    internal class CollisionAndLastStepPaddingAssetsRecord : AssetsRecord
    {
        public CollisionAndLastStepPaddingAssetsRecord() : base() { }
    }
}
