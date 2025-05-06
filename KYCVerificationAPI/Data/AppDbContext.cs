using KYCVerificationAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace KYCVerificationAPI.Data;

public class AppDbContext : DbContext
{
    public virtual DbSet<Verification> Verifications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Verification>(entity =>
        {
            modelBuilder.Entity<Verification>()
                .HasIndex(v => v.FirstName);
            
            modelBuilder.Entity<Verification>()
                .HasIndex(v => v.GivenName);            
            
            modelBuilder.Entity<Verification>()
                .HasIndex(v => v.Nin);
            
            modelBuilder.Entity<Verification>()
                .HasIndex(v => v.CardNumber);
            
            modelBuilder.Entity<Verification>()
                .HasIndex(v => v.CardNumber);
            
            modelBuilder.Entity<Verification>()
                .HasIndex(v => v.CreatedBy);
            
            modelBuilder.Entity<Verification>()
                .HasIndex(v => v.CreatedAt);
        });
    }
}