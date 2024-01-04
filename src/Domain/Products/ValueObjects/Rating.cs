using Domain.Errors;
using SharedKernel.Primitives;

namespace Domain.Products.ValueObjects;


public sealed class Rating : ValueObject
{
    public int Value { get; private set; }

    public Guid CustomerId { get; private set; }

    public static Result<Rating> Create(Guid customerId, int value)
    {
        if (value is < 0 or > 5)
        {
            return DomainError.Rating.InvalidRatingValue;
        }

        return new Rating(customerId, value);
    }

    private Rating(Guid customerId, int value)
    {
        Value = value;
        CustomerId = customerId;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
