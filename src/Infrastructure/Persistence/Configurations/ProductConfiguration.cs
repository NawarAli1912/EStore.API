﻿using Domain.Products;
using Domain.Products.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(TablesNames.Product);

        builder
            .HasKey(p => p.Id);

        builder.Property(p => p.CustomerPrice)
            .HasColumnType("decimal(12,2)");

        builder.Property(p => p.PurchasePrice)
            .HasColumnType("decimal(12,2)");

        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Products);

        builder.Property(p => p.Status)
            .HasDefaultValue(ProductStatus.Active);

        builder
            .HasMany(p => p.Reviews)
            .WithOne()
            .HasForeignKey(r => r.ProductId);

        var rowVersionProperty = typeof(Product).GetProperty("Version", BindingFlags.NonPublic | BindingFlags.Instance);

        builder
            .Property("_version")
            .HasColumnName("Version")
            .IsRowVersion();
    }
}
