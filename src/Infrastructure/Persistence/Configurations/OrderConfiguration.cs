using Domain.Customers;
using Domain.Orders;
using Domain.Products;
using Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .ToTable(TablesNames.Order);

        builder
            .HasKey(o => o.Id);

        builder
            .Property(o => o.Id)
            .ValueGeneratedNever();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .IsRequired();

        builder.OwnsMany(o => o.LineItems, liBuilder =>
        {
            liBuilder
                .HasKey("OrderId", "Id");

            liBuilder
                .Property(li => li.Id)
                .ValueGeneratedNever();

            liBuilder
                .WithOwner()
                .HasForeignKey("OrderId");

            liBuilder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(li => li.ProductId);

            liBuilder.Property(li => li.Price)
                .HasColumnType("decimal(12, 2)");

        });

        builder
            .ComplexProperty(o => o.ShippingInfo);

        builder
            .Property(o => o.TotalPrice)
            .HasPrecision(12, 2);

        builder
            .Property<List<Guid>>("_requestedOffers")
            .HasColumnName("RequestedOffers")
            .HasListOfIdsConverter();

        builder
            .HasIndex(o => o.Code)
            .IsUnique();
        builder.Metadata.FindNavigation(nameof(Order.LineItems))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
