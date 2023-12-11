using Domain.Orders.Entities;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> builder)
    {
        builder.ToTable(TablesNames.LineItem);

        builder.HasKey(li => li.Id);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(
                li => li.ProductId);

        builder.Property(li => li.Price)
            .HasColumnType("decimal(12, 2)");
    }
}
