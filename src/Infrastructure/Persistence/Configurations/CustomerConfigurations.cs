using Domain.Customers;
using Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class CustomerConfigurations : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .ToTable(TablesNames.Customer);

        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever();

        builder.HasOne(c => c.Cart)
            .WithOne()
            .HasForeignKey<Cart>(cart => cart.CustomerId);
    }
}
