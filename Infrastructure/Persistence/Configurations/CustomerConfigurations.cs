using Domain.Customers;
using Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class CustomerConfigurations : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable(TablesNames.Customer, Schemas.Customers);
        builder
            .HasKey(c => c.Id);

        builder.OwnsOne(b => b.Address, addressBuilder =>
        {
            addressBuilder
                .Property(a => a.City)
                .IsRequired();

            addressBuilder
                .Property(a => a.County)
                .IsRequired();
        });

        builder.HasOne(c => c.Cart)
            .WithOne()
            .HasForeignKey<Cart>(cart => cart.CustomerId);
    }
}
