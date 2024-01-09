using Domain.Categories;
using Domain.Errors;
using Domain.Products.Entities;
using Domain.Products.Enums;
using Domain.Products.Events;
using Domain.Products.ValueObjects;
using SharedKernel.Primitives;
using System.Collections.Immutable;
using Result = SharedKernel.Primitives.Result;

namespace Domain.Products;

public class Product : AggregateRoot
{
    private byte[] _version { get; set; }

    private readonly List<Category> _categories = [];

    private readonly HashSet<ProductReview> _reviews = [];

    private readonly HashSet<Rating> _ratings = [];

    private readonly List<Guid> _associatedOffers = [];

    public string Code { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal CustomerPrice { get; private set; } = default!;

    public decimal PurchasePrice { get; private set; } = default!;

    public ProductStatus Status { get; private set; }

    public int ViewCount { get; private set; }

    public IReadOnlyCollection<Category> Categories => _categories;

    public IReadOnlyCollection<ProductReview> Reviews => _reviews;

    public List<Guid> AssociatedOffers => _associatedOffers.ToList();

    public byte[] Version => _version;

    public double AverageRating => _ratings.Count > 0 ?
        _ratings.Select(r => r.Value).Sum() / _ratings.Count :
        0.0;

    public static Product Create(
        Guid id,
        string name,
        string description,
        int quantity,
        decimal customerPrice,
        decimal purchasePrice,
        string code = "",
        IEnumerable<Category>? categories = default,
        IEnumerable<Guid>? associatedOffers = default)
    {
        var product = new Product(
            id,
            name,
            description,
            quantity,
            customerPrice,
            purchasePrice,
            code);

        product.AssignCategories(categories ?? Array.Empty<Category>());

        product.AssociateOffers(associatedOffers ?? Array.Empty<Guid>());

        product.RaiseDomainEvent(
            new ProductCreatedDomainEvent(product));

        return product;
    }

    public void AssignCategories(IEnumerable<Category> categories)
    {
        foreach (var category in categories)
        {
            if (!_categories.Contains(category))
            {
                _categories.Add(category);
            }
        }

        RaiseDomainEvent(
            new ProductUpdatedDomainEvent(this));
    }

    public void UnassignCategories(IEnumerable<Category> categories)
    {
        foreach (var category in categories)
        {
            if (!categories.Contains(category))
            {
                continue;
            }

            _categories.Remove(category);
        }

        RaiseDomainEvent(
            new ProductUpdatedDomainEvent(this));
    }

    public Result<Product> Update(
        string? name,
        string? description,
        int? quantity,
        decimal? customerPrice,
        decimal? purchasePrice)
    {
        if (quantity < 0)
        {
            return DomainError.Products.StockError(Name);
        }

        Name = name ?? Name;
        Description = description ?? Description;
        Quantity = quantity ?? Quantity;
        CustomerPrice = customerPrice ?? CustomerPrice;
        PurchasePrice = purchasePrice ?? PurchasePrice;
        Status = quantity == 0 ? ProductStatus.OutOfStock : Status;

        RaiseDomainEvent(
            new ProductUpdatedDomainEvent(this));


        return this;
    }

    public Result<Updated> DecreaseQuantity(int value)
    {
        Quantity -= value;

        switch (Quantity)
        {
            case < 0:
                Quantity += value;
                return DomainError.Products.StockError(Name);
            case 0:
                Status = ProductStatus.OutOfStock;
                break;
        }

        RaiseDomainEvent(
            new ProductUpdatedDomainEvent(this));

        return Result.Updated;
    }

    public void IncreaseQuantity(int value)
    {
        Quantity += value;
        RaiseDomainEvent(
            new ProductUpdatedDomainEvent(this));

    }

    public void MarkAsDeleted()
    {
        Status = ProductStatus.Deleted;
        RaiseDomainEvent(
            new ProductUpdatedDomainEvent(this));
    }

    public void AssociateOffers(IEnumerable<Guid> offerIds)
    {
        foreach (var offerId in offerIds)
        {
            _associatedOffers.Add(offerId);
        }
        RaiseDomainEvent(new ProductUpdatedDomainEvent(this));
    }

    public void UnassociateOffers(IEnumerable<Guid> offerIds)
    {
        foreach (var offerId in offerIds)
        {
            _associatedOffers.Remove(offerId);
        }
        RaiseDomainEvent(new ProductUpdatedDomainEvent(this));
    }

    public void SetCode(string code)
    {
        Code = code;
    }

    private Product() : base(Guid.NewGuid())
    {
    }

    private Product(Guid id,
        string name,
        string description,
        int quantity,
        decimal customerPrice,
        decimal purchasePrice,
        string code = "") : base(id)
    {
        Name = name;
        Description = description;
        Quantity = quantity;
        CustomerPrice = customerPrice;
        PurchasePrice = purchasePrice;
        Status = ProductStatus.Active;
        Code = code;
        if (Quantity == 0)
        {
            Status = ProductStatus.OutOfStock;
        }
    }
}
