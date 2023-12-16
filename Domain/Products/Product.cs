using Domain.Categories;
using Domain.ModelsSnapshots;
using Domain.Products.Entities;
using Domain.Products.Enums;
using Domain.Products.Errors;
using Domain.Products.Events;
using Domain.Products.ValueObjects;
using SharedKernel;
using SharedKernel.Models;

namespace Domain.Products;

public class Product : AggregateRoot<Guid>
{
    private readonly List<Category> _categories = [];

    private readonly List<ProductReview> _reviews = [];

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal CustomerPrice { get; private set; } = default!;

    public decimal PurchasePrice { get; private set; } = default!;

    public Sku? Sku { get; private set; } = default;

    public ProductStatus Status { get; private set; }

    public IReadOnlyCollection<Category> Categories => _categories;

    public IReadOnlyCollection<ProductReview> Reviews => _reviews;

    public static Result<Product> Create(
        Guid id,
        string name,
        string description,
        int quantity,
        decimal customerPrice,
        decimal purchasePrice,
        string? sku = default,
        IEnumerable<Category>? categories = default)
    {
        List<Error> errors = [];

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
            CustomerPrice = customerPrice,
            PurchasePrice = purchasePrice,
            Sku = skuResult.Value
        };

        foreach (var category in categories ?? Array.Empty<Category>())
        {
            product.AssignCategory(category);
        }

        product.Status = ProductStatus.Active;
        if (product.Quantity == 0)
        {
            product.Status = ProductStatus.OutOfStock;
        }

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(ProductSnapshot.Snapshot(product)));

        return product;
    }

    public void AssignCategory(Category category)
    {
        if (_categories.Contains(category))
        {
            return;
        }

        _categories.Add(category);
    }

    public void UnassignCategory(Category category)
    {
        if (!_categories.Contains(category))
        {
            return;
        }

        _categories.Remove(category);
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
        var newCustomerPrice = customerPrice ?? CustomerPrice;
        var newPurchasePrice = purchasePrice ?? PurchasePrice;
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

        Name = name ?? Name;
        Description = description ?? Description;
        Quantity = quantity ?? Quantity;
        CustomerPrice = newCustomerPrice;
        PurchasePrice = newPurchasePrice;

        return this;
    }

    public Result<Updated> DecreaseQuantity(int value)
    {
        Quantity -= value;

        switch (Quantity)
        {
            case < 0:
                Quantity += value;
                return DomainError.Product.StockError(Name);
            case 0:
                Status = ProductStatus.OutOfStock;
                break;
        }

        return Result.Updated;
    }

    public void IncreaseQuantity(int value)
    {
        Quantity += value;
    }

    public void MarkAsDeleted()
    {
        Status = ProductStatus.Deleted;
        RaiseDomainEvent(new ProductUpdatedDomainEvent(ProductSnapshot.Snapshot(this)));
    }

    private Product() : base(Guid.NewGuid())
    {
    }

    private Product(Guid id) : base(id) { }
}
