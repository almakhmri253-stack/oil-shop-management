using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Models;

namespace OilShopManagement.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnItem> PurchaseReturnItems => Set<PurchaseReturnItem>();
    public DbSet<SaleReturn> SaleReturns => Set<SaleReturn>();
    public DbSet<SaleReturnItem> SaleReturnItems => Set<SaleReturnItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Invoice>()
            .HasOne(i => i.Customer)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<InvoiceItem>()
            .HasOne(ii => ii.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<InvoiceItem>()
            .HasOne(ii => ii.Product)
            .WithMany(p => p.InvoiceItems)
            .HasForeignKey(ii => ii.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockTransaction>()
            .HasOne(st => st.Product)
            .WithMany(p => p.StockTransactions)
            .HasForeignKey(st => st.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Purchase>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Purchases)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Purchase)
            .WithMany(p => p.Items)
            .HasForeignKey(pi => pi.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Product)
            .WithMany()
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseReturn>()
            .HasOne(pr => pr.Purchase)
            .WithMany(p => p.Returns)
            .HasForeignKey(pr => pr.PurchaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<PurchaseReturn>()
            .HasOne(pr => pr.Supplier)
            .WithMany(s => s.PurchaseReturns)
            .HasForeignKey(pr => pr.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<PurchaseReturnItem>()
            .HasOne(pri => pri.PurchaseReturn)
            .WithMany(pr => pr.Items)
            .HasForeignKey(pri => pri.PurchaseReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PurchaseReturnItem>()
            .HasOne(pri => pri.Product)
            .WithMany()
            .HasForeignKey(pri => pri.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SaleReturn>()
            .HasOne(sr => sr.Invoice)
            .WithMany()
            .HasForeignKey(sr => sr.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<SaleReturn>()
            .HasOne(sr => sr.Customer)
            .WithMany()
            .HasForeignKey(sr => sr.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<SaleReturnItem>()
            .HasOne(sri => sri.SaleReturn)
            .WithMany(sr => sr.Items)
            .HasForeignKey(sri => sri.SaleReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SaleReturnItem>()
            .HasOne(sri => sri.Product)
            .WithMany()
            .HasForeignKey(sri => sri.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "ط²ظٹطھ ظ…ط­ط±ظƒ", Description = "ط¬ظ…ظٹط¹ ط£ظ†ظˆط§ط¹ ط²ظٹظˆطھ ط§ظ„ظ…ط­ط±ظƒ", IsActive = true, CreatedAt = seedDate },
            new Category { Id = 2, Name = "ط²ظٹطھ ظ‚ظٹط±", Description = "ط²ظٹظˆطھ ظ†ط§ظ‚ظ„ ط§ظ„ط­ط±ظƒط©", IsActive = true, CreatedAt = seedDate },
            new Category { Id = 3, Name = "ظپظ„ط§طھط±", Description = "ظپظ„ط§طھط± ط§ظ„ط²ظٹطھ ظˆط§ظ„ظ‡ظˆط§ط،", IsActive = true, CreatedAt = seedDate },
            new Category { Id = 4, Name = "ط¥ط¶ط§ظپط§طھ", Description = "ط¥ط¶ط§ظپط§طھ ظˆظ…ظˆط§ط¯ طھط´ط­ظٹظ…", IsActive = true, CreatedAt = seedDate }
        );
    }
}


