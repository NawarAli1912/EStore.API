using Domain.Orders.Entities;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> builder)
    {
        builder.ToTable(TablesNames.LineItem, Schemas.Orders);

        builder.HasKey(li => li.Id);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(
                li => li.ProductId);

        builder.ComplexProperty(li => li.Price, priceConfiguration =>
        {
            priceConfiguration.Property(p => p.Currency)
                    .HasMaxLength(3);

            priceConfiguration.Property(p => p.Value)
                    .HasColumnType("decimal(12, 2)");
        });
    }
}
