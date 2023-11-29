using Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable(TablesNames.Cart, Schemas.Carts);

        builder
            .HasKey(c => c.Id);

        builder.OwnsMany(c => c.CartItems, cartItemsBuilder =>
        {
            cartItemsBuilder.ToTable(TablesNames.CartItemsTable, Schemas.Carts);
        });
    }
}
