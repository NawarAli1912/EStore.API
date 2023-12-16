using SharedKernel;
using SharedKernel.Models;

namespace Domain.Products.ValueObjects;

public sealed class Sku : ValueObject
{
    private const int DefaultLength = 15;

    private Sku(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static explicit operator string(Sku? sku) => sku is null ? "" : sku.Value;

    public static Result<Sku?> Create(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? Result.From<Sku?>(null) :
            // More validation if needed
            new Sku(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
