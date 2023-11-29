using Domain.Kernal.Models;

namespace Domain.Customers.ValueObjects;

public sealed class Address : ValueObject
{
    public string? Building { get; private set; } = default;

    public string? Street { get; private set; } = default;

    public string? City { get; private set; } = default;

    public string? County { get; private set; } = default;

    public string? PostalCode { get; private set; } = default;

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Building ?? "";
        yield return Street ?? "";
        yield return City ?? "";
        yield return County ?? "";
        yield return PostalCode ?? "";
    }

    public static Address Create(
        string? city = null,
        string? county = null,
        string? postalCode = null,
        string? building = null,
        string? street = null)
    {
        return new Address
        {
            Building = building,
            Street = street,
            City = city,
            County = county,
            PostalCode = postalCode
        };
    }
}