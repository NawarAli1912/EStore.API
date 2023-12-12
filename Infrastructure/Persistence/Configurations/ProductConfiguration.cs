using Domain.Products;
using Domain.Products.Enums;
using Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(TablesNames.Product);

        builder
            .HasKey(p => p.Id);

        builder.Property(p => p.Sku)
            .HasConversion(
                sku => (sku == null) ? null : sku.Value,
                value => Sku.Create(value).Value)
            .IsRequired(false);

        builder.Property(p => p.CustomerPrice)
            .HasColumnType("decimal(12,2)");

        builder.Property(p => p.PurchasePrice)
            .HasColumnType("decimal(12,2)");

        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Products);

        builder.Property(p => p.Status)
            .HasDefaultValue(ProductStatus.Active);
    }
}
