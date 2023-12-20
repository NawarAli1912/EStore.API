using Domain.Categories;
using Domain.Customers;
using Domain.ModelsSnapshots;
using Domain.Products.Entities;
using Domain.Products.Enums;
using Domain.Products.Errors;
using Domain.Products.Events;
using Domain.Products.ValueObjects;
using SharedKernel;
using SharedKernel.Models;
using Result = SharedKernel.Result;

namespace Domain.Products;

public class Product : AggregateRoot<Guid>
{
    private readonly List<Category> _categories = [];

    private readonly HashSet<ProductReview> _reviews = [];

    private readonly HashSet<Rating> _ratings = [];

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal CustomerPrice { get; private set; } = default!;

    public decimal PurchasePrice { get; private set; } = default!;

    public Sku? Sku { get; private set; } = default;

    public ProductStatus Status { get; private set; }

    public IReadOnlyCollection<Category> Categories => _categories;

    public IReadOnlyCollection<ProductReview> Reviews => _reviews;

    public double AverageRating => _ratings.Count > 0 ? _ratings.Select(r => r.Value).Sum() /
                            _ratings.Count : 0.0;

    public static Result<Product> Create(
        Guid id,
        string name,
        string description,
        int quantity,
        decimal customerPrice,
        decimal purchasePrice,
        Sku? sku = default,
        IEnumerable<Category>? categories = default)
    {
        List<Error> errors = [];

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
            Sku = sku
        };


        product.AssignCategories(categories ?? Array.Empty<Category>());


        product.Status = ProductStatus.Active;
        if (product.Quantity == 0)
        {
            product.Status = ProductStatus.OutOfStock;
        }

        product.RaiseDomainEvent(
            new ProductCreatedDomainEvent(ProductSnapshot.Snapshot(product)));

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
    }

    public Result<Product> Update(
        string? name,
        string? description,
        int? quantity,
        decimal? customerPrice,
        decimal? purchasePrice,
        Sku? sku,
        bool nullSku = false)
    {
        List<Error> errors = [];
        Sku = null;
        if (!nullSku)
        {
            Sku = sku;
        }

        if (quantity < 0)
        {
            errors.Add(DomainError.Product.StockError(Name));
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        Name = name ?? Name;
        Description = description ?? Description;
        Quantity = quantity ?? Quantity;
        CustomerPrice = customerPrice ?? CustomerPrice;
        PurchasePrice = purchasePrice ?? PurchasePrice;
        Status = quantity == 0 ? ProductStatus.OutOfStock : Status;

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

    public void AddReview(ProductReview review)
    {
        _reviews.Add(review);
    }

    public void RemoveReview(Guid reviewId)
    {
        var productReview = ProductReview.Create(reviewId);

        _reviews.Remove(productReview);
    }

    public void UpdateReview(Guid reviewId, ProductReview newReview)
    {
        var oldReview = ProductReview
            .Create(reviewId);

        if (!_reviews.TryGetValue(oldReview, out oldReview))
        {
            return;
        }

        oldReview.UpdateComment(newReview.Comment);
    }

    public Result<Updated> AddRating(Customer customer, int rating)
    {
        var customerRating = Rating.Create(customer.Id, rating);

        if (customerRating.IsError)
        {
            return customerRating.Errors;
        }

        _ratings.Add(customerRating.Value);

        return Result.Updated;
    }

    public Result<Updated> RemoveRating(Customer customer, int rating)
    {
        var customerRating = Rating.Create(customer.Id, rating);

        if (customerRating.IsError)
            return customerRating.Errors;

        if (!_ratings.TryGetValue(customerRating.Value, out var oldRating))
        {
            return DomainError.Rating.NotFound;
        }

        _ratings.Remove(oldRating);

        return Result.Updated;
    }

    private Product() : base(Guid.NewGuid())
    {
    }

    private Product(Guid id) : base(id)
    {
    }
}
