namespace SharedKernel.Primitives;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && Equals(other);
    }

    public static bool operator ==(ValueObject left, ValueObject right) =>
        left.Equals(right);

    public static bool operator !=(ValueObject left, ValueObject right) =>
        !left.Equals(right);

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(property => property?.GetHashCode() ?? 0)
            .Aggregate((a, b) => a ^ b);
    }

    public bool Equals(ValueObject? other)
    {
        if (other is null)
        {
            return false;
        }

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }
}
