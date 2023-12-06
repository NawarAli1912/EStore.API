using Domain.Products;

namespace Domain.ModelsSnapshots;

public sealed class ProductSnapshot
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal PurchasePrice_Value { get; set; }

    public decimal CustomerPrice_Value { get; set; }

    public string Sku { get; set; } = default!;

    [Nest.Ignore]
    public int PurchasePrice_Currency { get; set; }

    [Nest.Ignore]
    public int CustomerPrice_Currency { get; set; }


    public static ProductSnapshot Snapshot(Product product)
    {
        var item = new ProductSnapshot
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            PurchasePrice_Currency = (int)product.PurchasePrice.Currency,
            PurchasePrice_Value = product.PurchasePrice.Value,
            Sku = product.Sku is null ? string.Empty : product.Sku.Value,
            CustomerPrice_Currency = (int)product.CustomerPrice.Currency,
            CustomerPrice_Value = product.CustomerPrice.Value
        };

        return item;
    }
}
