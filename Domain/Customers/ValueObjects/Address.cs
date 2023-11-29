using Domain.Kernal.Models;

namespace Domain.Customers.ValueObjects;

public class Address : ValueObject
{
    public string? Building { get; private set; } = null;

    public string? Street { get; private set; } = null;

    public string City { get; private set; } = string.Empty;

    public string County { get; private set; } = string.Empty;

    public string PostalCode { get; private set; } = string.Empty;

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Building ?? "";
        yield return Street ?? "";
        yield return City;
        yield return County;
        yield return PostalCode;
    }

    public static Address Create(
        string city,
        string county,
        string postalCode,
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