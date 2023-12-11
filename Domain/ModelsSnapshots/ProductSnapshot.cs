﻿using Domain.Products;

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
        };

        return item;
    }
}
