using Domain.Categories;
using Domain.Errors;
using Domain.Products.Entities;
using Domain.Products.Enums;
using Domain.Products.Events;
using Domain.Products.ValueObjects;
using SharedKernel.Primitives;
using Result = SharedKernel.Primitives.Result;

namespace Domain.Products;

public class Product : AggregateRoot
{
    private byte[] _version { get; set; }

    private readonly List<Category> _categories = [];

    private readonly HashSet<ProductReview> _reviews = [];

    private readonly HashSet<Rating> _ratings = [];

    private readonly List<Guid> _associatedOffers = [];

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal CustomerPrice { get; private set; } = default!;

    public decimal PurchasePrice { get; private set; } = default!;

    public ProductStatus Status { get; private set; }

    public int ViewCount { get; private set; }

    public IReadOnlyCollection<Category> Categories => _categories;

    public IReadOnlyCollection<ProductReview> Reviews => _reviews;

    public IReadOnlyList<Guid> AssociatedOffers => _associatedOffers;

    public double AverageRating => _ratings.Count > 0 ? _ratings.Select(r => r.Value).Sum() /
                            _ratings.Count : 0.0;

    public static Product Create(
        Guid id,
        string name,
        string description,
        int quantity,
        decimal customerPrice,
        decimal purchasePrice,
        IEnumerable<Category>? categories = default)
    {
        List<Error> errors = [];

        var product = new Product
        {
            Id = id,
            Name = name,
            Description = description,
            Quantity = quantity,
            CustomerPrice = customerPrice,
            PurchasePrice = purchasePrice,
        };


        product.AssignCategories(categories ?? Array.Empty<Category>());


        product.Status = ProductStatus.Active;
        if (product.Quantity == 0)
        {
            product.Status = ProductStatus.OutOfStock;
        }

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
            return DomainError.Product.StockError(Name);
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
                return DomainError.Product.StockError(Name);
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

    public void AssociateOffer(Guid offerId)
    {
        _associatedOffers.Add(offerId);
    }

    public void UnassociateOffer(Guid offerId)
    {
        _associatedOffers.Remove(offerId);
    }

    private Product() : base(Guid.NewGuid())
    {
    }

    private Product(Guid id) : base(id)
    {
    }
}
