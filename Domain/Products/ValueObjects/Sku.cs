using Domain.Kernal.Models;

namespace Domain.Products.ValueObjects;

public class Sku : ValueObject
{
    private const int DefaultLength = 15;

    private Sku(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public static Sku? Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (value.Length != DefaultLength)
        {
            return null;
        }

        return new Sku(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
