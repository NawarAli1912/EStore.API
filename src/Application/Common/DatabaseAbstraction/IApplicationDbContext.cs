using Domain.Categories;
using Domain.Customers;
using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SharedKernel.Primitives;

namespace Application.Common.DatabaseAbstraction;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }

    DbSet<Category> Categories { get; set; }

    DbSet<Order> Orders { get; set; }

    DbSet<Customer> Customers { get; set; }

    DbSet<Offer> Offers { get; set; }

    ChangeTracker ChangeTracker { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();

    void MarkAdded(Entity entity);

    void MarkModified(Entity entity);

    void MarkRemoved(Entity entity);
}
