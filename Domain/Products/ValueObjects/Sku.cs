﻿using Domain.Kernal;
using Domain.Kernal.Models;

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
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.From<Sku?>(null);
        }
        /*
        if (value.Length != DefaultLength)
        {
            return Errors.Sku.InvalidLength;
        }*/

        return new Sku(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
