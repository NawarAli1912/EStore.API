﻿using Domain.Errors;
using SharedKernel.Primitives;

namespace Domain.Customers.ValueObjects;

public sealed class CartItem : ValueObject
{
    public Guid CartId { get; init; }

    public Guid ItemId { get; init; }

    public int Quantity { get; init; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ItemId;
    }

    internal static Result<CartItem> Create(
        Guid id,
        Guid ItemId,
        int quantity)
    {
        if (quantity < 0)
        {
            return DomainError.CartItems.NegativeQuantity;
        }

        return new CartItem
        {
            CartId = id,
            ItemId = ItemId,
            Quantity = quantity
        };
    }
}
