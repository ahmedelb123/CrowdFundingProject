using Microsoft.EntityFrameworkCore;
using System;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Entity Configuration
        modelBuilder.Entity<User>()
            .Property(u => u.createdAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAdd();

        // Post Entity Configuration
        modelBuilder.Entity<Post>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Post>()
            .Property(p => p.UserId)
            .IsRequired();

        modelBuilder.Entity<Post>()
            .Property(p => p.Title)
            .IsRequired();

        modelBuilder.Entity<Post>()
            .Property(p => p.Content)
            .IsRequired();

        modelBuilder.Entity<Post>()
            .Property(p => p.MediaUrl)
            .IsRequired();

        modelBuilder.Entity<Post>()
            .Property(p => p.AmountGained)
            .HasDefaultValue(0);

        modelBuilder.Entity<Post>()
            .Property(p => p.CreatedAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        modelBuilder.Entity<Post>()
            .Property(p => p.UpdatedAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAddOrUpdate();

        // Donation Entity Configuration
        modelBuilder.Entity<Donation>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Donation>()
            .Property(d => d.UserId)
            .IsRequired();

        modelBuilder.Entity<Donation>()
            .Property(d => d.Amount)
            .IsRequired();

        modelBuilder.Entity<Donation>()
            .Property(d => d.CreatedAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // BankAccount Entity Configuration
        modelBuilder.Entity<BankAccount>()
            .Property(b => b.CreatedAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<BankAccount>()
            .Property(b => b.CardNumber)
            .HasColumnType("BIGINT")
            .IsRequired();

        modelBuilder.Entity<BankAccount>()
            .Property(b => b.SecretNumber)
            .IsRequired();
    }
}
