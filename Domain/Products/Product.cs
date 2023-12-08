﻿using Domain.Categories;
using Domain.DomainEvents;
using Domain.Kernal;
using Domain.Kernal.Models;
using Domain.Kernal.ValueObjects;
using Domain.ModelsSnapshots;
using Domain.Products.ValueObjects;

namespace Domain.Products;

public class Product : AggregateRoot<Guid>
{
    private readonly List<Category> _cateogries = [];

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public Money CustomerPrice { get; private set; } = default!;

    public Money PurchasePrice { get; private set; } = default!;

    public Sku? Sku { get; private set; } = default;

    public IReadOnlyList<Category> Categories => _cateogries.ToList();

    public static Result<Product> Create(
        Guid id,
        string name,
        string description,
        int quantity,
        decimal customerPrice,
        decimal purchasePrice,
        string currency,
        string? sku = default,
        IEnumerable<Category>? categories = default)
    {
        List<Error> errors = [];
        var customerPriceResult = Money.Create(customerPrice, currency);
        if (customerPriceResult.IsError)
        {
            errors.AddRange(customerPriceResult.Errors);
        }

        var purchasePriceResult = Money.Create(purchasePrice, currency);
        if (purchasePriceResult.IsError)
        {
            errors.AddRange(purchasePriceResult.Errors);
        }

        var skuResult = Sku.Create(sku);
        if (skuResult.IsError)
        {
            errors.AddRange(skuResult.Errors);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        var product = new Product
        {
            Id = id,
            Name = name,
            Description = description,
            Quantity = quantity,
            CustomerPrice = customerPriceResult.Value,
            PurchasePrice = purchasePriceResult.Value,
            Sku = skuResult.Value
        };

        foreach (var category in categories ?? Array.Empty<Category>())
        {
            product.AssignCategory(category);
        }

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(ProductSnapshot.Snapshot(product)));

        return product;
    }

    public void AssignCategory(Category category)
    {
        if (_cateogries.Contains(category))
        {
            return;
        }

        _cateogries.Add(category);
    }

    public void UnassignCategory(Category category)
    {
        if (!_cateogries.Contains(category))
        {
            return;
        }

        _cateogries.Remove(category);
    }

    public Result<Product> Update(
        string? name,
        string? description,
        int? quantity,
        decimal? customerPrice,
        decimal? purchasePrice,
        string? sku,
        bool nullSku = false)
    {
        List<Error> errors = [];
        var customerPriceResult = Money.Create(customerPrice ?? CustomerPrice.Value);
        var purchasePriceResult = Money.Create(purchasePrice ?? PurchasePrice.Value);
        Sku = null;
        if (!nullSku)
        {
            var skuResult = Sku.Create(sku ?? Sku?.Value);
            errors.AddRange(skuResult.Errors);
            if (!skuResult.IsError)
            {
                Sku = skuResult.Value;
            }
        }

        errors.AddRange(customerPriceResult.Errors);
        errors.AddRange(purchasePriceResult.Errors);

        if (errors.Count > 0)
        {
            return errors;
        }

        Name = name ?? Name;
        Description = description ?? Description;
        Quantity = quantity ?? Quantity;
        CustomerPrice = customerPriceResult.Value;
        PurchasePrice = purchasePriceResult.Value;

        return this;
    }

    private Product() : base(Guid.NewGuid())
    {
    }

    private Product(Guid id) : base(id) { }
}
