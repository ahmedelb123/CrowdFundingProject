using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Donation> Donations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.createdAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Donation>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Donation>()
            .Property(d => d.UserId)
            .IsRequired();

        modelBuilder.Entity<Donation>()
            .Property(d => d.Amount)
            .IsRequired();

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

        modelBuilder.Entity<User>()
    .Property(u => u.createdAt)
    .HasColumnType("timestamp")
    .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<User>(entity =>
    {
        entity.Property(e => e.createdAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAdd();
    });
    
    modelBuilder.Entity<Donation>()
        .HasOne<Post>(d => d.Post)
        .WithMany(p => p.Donations) 
        .HasForeignKey(d => d.PostId); 
    }
}