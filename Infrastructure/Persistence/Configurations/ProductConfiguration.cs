using Domain.Products;
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

        builder.ComplexProperty(p => p.CustomerPrice, priceBuilder =>
        {
            priceBuilder.Property(m => m.Currency)
                        .HasMaxLength(3);

            priceBuilder.Property(m => m.Value)
                        .HasColumnType("decimal(12,2)");
        });

        builder.ComplexProperty(p => p.PurchasePrice, priceBuilder =>
        {
            priceBuilder.Property(m => m.Currency)
                        .HasMaxLength(3);

            priceBuilder.Property(m => m.Value)
                        .HasColumnType("decimal(12,2)");
        });

        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Products);
    }
}
