using Domain.Carts;
using Domain.Customers;
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

        builder
            .HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.Name)
            .HasMaxLength(256);

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.HasOne<Cart>()
            .WithOne()
            .HasForeignKey<Cart>(c => c.CustomerId);
    }
}
