using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .ToTable(TablesNames.Product);

        builder
            .HasKey(p => p.Id);

        builder.Property(p => p.CustomerPrice)
            .HasColumnType("decimal(12,2)");

        builder.Property(p => p.PurchasePrice)
            .HasColumnType("decimal(12,2)");

        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Products);

        builder
            .HasMany(p => p.Reviews)
            .WithOne()
            .HasForeignKey(r => r.ProductId);

        builder
            .Property("_version")
            .HasColumnName("Version")
        .IsRowVersion();

        builder
            .Property(p => p.AssociatedOffers)
            .HasField("_associatedOffers");

        builder
            .HasIndex(p => p.Code)
            .IsUnique();
    }
}
