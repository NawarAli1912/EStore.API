using Application.Common.Data;
using Domain.Categories;
using Domain.Customers;
using Domain.Orders;
using Domain.Products;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<Category> Categories { get; set; } = default!;

    public DbSet<Order> Orders { get; set; } = default!;

    public DbSet<Customer> Customers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
