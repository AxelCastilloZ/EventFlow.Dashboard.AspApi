using EventFlow.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventFlow.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets - Matching Grupo3 database tables
    public DbSet<User> Users { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchaseDetail> PurchaseDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.User_Id);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.First_Name).IsRequired();
            entity.Property(e => e.Last_Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Password).IsRequired();
        });

        // Configure Card entity
        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("Cards");
            entity.HasKey(e => e.Card_Id);
            entity.Property(e => e.Card_Type).IsRequired();
            entity.Property(e => e.Card_Number).IsRequired();
            entity.Property(e => e.Money).IsRequired();
            entity.Property(e => e.Expiration_Date).IsRequired();
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Product_Id);
            entity.Property(e => e.Product_Name).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Price).IsRequired();
        });

        // Configure Purchase entity
        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable("Purchases");
            entity.HasKey(e => e.Purchase_Id);
            entity.Property(e => e.SubTotal).IsRequired();
            entity.Property(e => e.Purchase_Date).IsRequired();

            // Relationship: Purchase belongs to a Card
            entity.HasOne(e => e.Card)
                  .WithMany(c => c.Purchases)
                  .HasForeignKey(e => e.Card_Id)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Purchase has many PurchaseDetails
            entity.HasMany(e => e.PurchaseDetails)
                  .WithOne(pd => pd.Purchase)
                  .HasForeignKey(pd => pd.Purchase_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PurchaseDetail entity
        modelBuilder.Entity<PurchaseDetail>(entity =>
        {
            entity.ToTable("PurchaseDetails");
            entity.HasKey(e => e.Purchase_Detail_Id);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Total).IsRequired();
            entity.Property(e => e.Purchase_Date).IsRequired();

            // Relationship: PurchaseDetail belongs to a Purchase
            entity.HasOne(e => e.Purchase)
                  .WithMany(p => p.PurchaseDetails)
                  .HasForeignKey(e => e.Purchase_Id)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relationship: PurchaseDetail belongs to a Product
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.PurchaseDetails)
                  .HasForeignKey(e => e.Product_Id)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}