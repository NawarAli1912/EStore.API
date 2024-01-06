using Domain.Products;
using Domain.Products.Enums;

namespace Domain.ModelsSnapshots;

public sealed class ProductSnapshot
{
    public Guid Id { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal CustomerPrice { get; set; }

    public int ViewCount { get; set; }

    public ProductStatus Status { get; set; }

    public List<CategorySnapshot> Categories { get; set; } = [];

    public List<Guid> AssociatedOffers { get; set; } = [];

    public static ProductSnapshot Snapshot(Product product)
    {
        var item = new ProductSnapshot
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            PurchasePrice = product.PurchasePrice,
            CustomerPrice = product.CustomerPrice,
            ViewCount = product.ViewCount,
            Status = product.Status,
            AssociatedOffers = [.. product.AssociatedOffers]
        };

        foreach (var category in product.Categories)
        {
            item.Categories.Add(CategorySnapshot.Snapshot(category));
        }

        return item;
    }

    public static bool Equals(ProductSnapshot lhs, ProductSnapshot rhs)
    {
        return lhs.Id == rhs.Id
            && lhs.Code == rhs.Code
            && lhs.Name == rhs.Name
            && lhs.Description == rhs.Description
            && lhs.Quantity == rhs.Quantity
            && lhs.PurchasePrice.Equals(rhs.PurchasePrice)
            && lhs.CustomerPrice.Equals(rhs.CustomerPrice)
            && lhs.Categories.OrderBy(c => c.CategoryId)
                    .SequenceEqual(rhs.Categories.OrderBy(c => c.CategoryId));
    }
}
