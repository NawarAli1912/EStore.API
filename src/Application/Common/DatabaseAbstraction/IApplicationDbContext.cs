using Domain.Categories;
using Domain.Customers;
using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.DatabaseAbstraction;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }

    DbSet<Category> Categories { get; set; }

    DbSet<Order> Orders { get; set; }

    DbSet<Customer> Customers { get; set; }

    DbSet<Offer> Offers { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}
