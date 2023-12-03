namespace Infrastructure.Persistence.ModelsSnapshots;

public class ProductRecord
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public int Quantity { get; set; }

    public int PurchasePrice_Currency { get; set; }

    public decimal PurchasePrice_Value { get; set; }

    public string Sku { get; set; } = default!;

    public int CustomerPrice_Currency { get; set; }

    public decimal CustomerPrice_Value { get; set; }
}
