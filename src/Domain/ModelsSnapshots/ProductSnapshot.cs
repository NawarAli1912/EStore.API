using Domain.Products;
using Domain.Products.Enums;

namespace Domain.ModelsSnapshots;

public sealed class ProductSnapshot
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal CustomerPrice { get; set; }

    public string Sku { get; set; } = default!;

    public int ViewCount { get; set; }

    public ProductStatus Status { get; set; }

    public List<CategorySnapshot> Categories { get; set; } = [];

    public static ProductSnapshot Snapshot(Product product)
    {
        var item = new ProductSnapshot
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            PurchasePrice = product.PurchasePrice,
            CustomerPrice = product.CustomerPrice,
            Sku = product.Sku is null ? string.Empty : product.Sku.Value,
            ViewCount = product.ViewCount,
            Status = product.Status
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
            && lhs.Name == rhs.Name
            && lhs.Description == rhs.Description
            && lhs.Quantity == rhs.Quantity
            && lhs.PurchasePrice.Equals(rhs.PurchasePrice)
            && lhs.CustomerPrice.Equals(rhs.CustomerPrice)
            && lhs.Sku.Equals(rhs.Sku)
            && lhs.Categories.OrderBy(c => c.CategoryId)
                    .SequenceEqual(rhs.Categories.OrderBy(c => c.CategoryId));
    }
}
