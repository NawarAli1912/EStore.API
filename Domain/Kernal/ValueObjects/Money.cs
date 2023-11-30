
using Domain.DomainErrors.Common;
using Domain.Kernal.Models;

namespace Domain.Kernal.ValueObjects;

public class Money : ValueObject, IComparable, IComparable<Money>
{
    private Money(decimal value, Currency currency)
    {
        Value = value;
        Currency = currency;
    }

    public decimal Value { get; private set; }

    public Currency Currency { get; private set; }

    public static Result<Money> Create(decimal value, string currency)
    {
        if (!Enum.TryParse<Currency>(currency.ToUpper(), out var parsedCurrency))
        {
            return Errors.Money.InvalidCurrency;
        }

        return new Money(value, parsedCurrency);
    }

    public int CompareTo(Money? other)
    {
        AssertSameCurrency(this, other);
        if (other is null)
            return 1;
        if (Value < other.Value)
            return -1;
        if (Value > other.Value)
            return 1;
        return 0;
    }

    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }
        if (obj is not Money)
        {
            throw new ArgumentException("Object is not Money object");
        }

        return CompareTo((Money)obj);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Currency;
    }

    #region operator
    public static bool operator >(Money left, Money right)
    {
        AssertSameCurrency(left, right);
        return left.Value > right.Value;
    }

    public static bool operator >=(Money left, Money right)
    {
        AssertSameCurrency(left, right);
        return left.Value >= right.Value;
    }

    public static bool operator <(Money left, Money right)
    {
        AssertSameCurrency(left, right);
        return left.Value < right.Value;
    }

    public static bool operator <=(Money left, Money right)
    {
        AssertSameCurrency(left, right);
        return left.Value <= right.Value;
    }

    public static Money operator *(Money left, decimal right)
    {
        AssertNotNull(left);
        return new Money(left.Value * right, left.Currency);
    }

    public static Money operator *(Money left, Money right)
    {
        AssertNotNull(left);
        return new Money(left.Value * right.Value, left.Currency);
    }

    public static Money operator /(Money left, decimal right)
    {
        AssertNotNull(left);
        AssertNotZero(right);
        return new Money(left.Value / right, left.Currency);
    }

    public static Money operator /(Money left, Money right)
    {
        AssertNotNull(left);
        AssertNotZero(right);
        return new Money(left.Value / right.Value, left.Currency);
    }

    public static Money operator +(Money left, Money right)
    {
        AssertSameCurrency(left, right);
        return new Money(left.Value + right.Value, left.Currency);
    }

    public static Money operator +(Money left, decimal right)
    {
        AssertNotNull(left);
        return new Money(left.Value + right, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        AssertSameCurrency(left, right);
        return new Money(left.Value - right.Value, left.Currency);
    }

    public static Money operator -(Money left, decimal right)
    {
        AssertNotNull(left);
        return new Money(left.Value - right, left.Currency);
    }
    #endregion

    private static void AssertNotNull(Money money)
    {
        ArgumentNullException.ThrowIfNull(money);
    }

    private static void AssertNotZero(Money money)
    {
        if (money.Value == 0)
        {
            throw new ArgumentException("Trying to divide by 0.");
        }
    }

    private static void AssertNotZero(decimal value)
    {
        if (value == 0)
        {
            throw new ArgumentException("Trying to divide by 0.");
        }
    }

    private static void AssertSameCurrency(Money first, Money second)
    {
        if (first is null || second is null)
            throw new ArgumentNullException("Any Money Is Null");
        if (first.Currency != second.Currency)
            throw new ArgumentException("Money Currency Not Equal");
    }

}
