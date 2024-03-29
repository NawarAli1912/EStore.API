﻿using Application.Common.DatabaseAbstraction;
using Domain.Authentication;
using Domain.Categories;
using Domain.Customers;
using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Infrastructure.Persistence.FriendlyIdentifiers;
using Infrastructure.Persistence.Outbox;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Primitives;

namespace Infrastructure.Persistence;

public class ApplicationDbContext :
    IdentityDbContext<IdentityUser, Role, string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    private IDbContextTransaction? _currentTransaction;

    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<Category> Categories { get; set; } = default!;

    public DbSet<Order> Orders { get; set; } = default!;

    public DbSet<Customer> Customers { get; set; } = default!;

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;

    public DbSet<Permission> Permissions { get; set; } = default!;

    public DbSet<Offer> Offers { get; set; } = default!;

    public DbSet<FriendlyIdSequence> FriendlyIdSequences { get; set; } = default!;

    ChangeTracker IApplicationDbContext.ChangeTracker { get => ChangeTracker; }

    public void MarkAdded(Entity entity)
    {
        Entry(entity).State = EntityState.Added;
    }

    public void MarkRemoved(Entity entity)
    {
        Entry(entity).State = EntityState.Deleted;
    }

    public void MarkModified(Entity entity)
    {
        Entry(entity).State = EntityState.Modified;
    }

    public async Task BeginTransactionAsync()
    {
        _currentTransaction = await Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            await _currentTransaction!.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _currentTransaction!.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _currentTransaction!.RollbackAsync();
        _currentTransaction.Dispose();
        _currentTransaction = null;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);

        base.OnModelCreating(modelBuilder);
    }


}
