using Microsoft.EntityFrameworkCore;
using Module14_ORM.Domain;

namespace Module14_ORM.EfCore;

public sealed class OrmDbContext(DbContextOptions<OrmDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Name).IsRequired().HasMaxLength(200);
            entity.Property(product => product.Description).HasMaxLength(1000);
            entity.Property(product => product.Weight).HasPrecision(18, 2);
            entity.Property(product => product.Height).HasPrecision(18, 2);
            entity.Property(product => product.Width).HasPrecision(18, 2);
            entity.Property(product => product.Length).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(order => order.Id);
            entity.Property(order => order.Status).IsRequired();
            entity.Property(order => order.CreatedDate).IsRequired();
            entity.Property(order => order.UpdatedDate).IsRequired();
            entity.HasOne(order => order.Product)
                .WithMany()
                .HasForeignKey(order => order.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}