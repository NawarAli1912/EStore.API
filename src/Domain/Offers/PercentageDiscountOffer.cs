using Domain.Offers.Enums;

namespace Domain.Offers;

public sealed class PercentageDiscountOffer : Offer
{
    public Guid ProductId { get; private set; }

    public decimal Discount { get; private set; }

    public static PercentageDiscountOffer Create(
        string name,
        string description,
        Guid productId,
        decimal percentage,
        DateOnly startDate,
        DateOnly endDate)
    {
        return new PercentageDiscountOffer
        {
            Name = name,
            Description = description,
            ProductId = productId,
            Discount = percentage,
            Type = OfferType.PercentageDiscountOffer,
            StartsAt = startDate,
            EndsAt = endDate
        };
    }

    /// <summary>
    /// Calculates the total price based on the products and their respective prices.
    /// This method iterates through each product in the provided dictionary and sums up their prices.
    /// </summary>
    /// <param name="products">A dictionary representing products, where the key is the product's unique identifier (ProductId) and the value is the product's price.</param>
    /// <returns>The total price as a decimal, representing the sum of the prices of all products in the dictionary.</returns>
    public override decimal CalculatePrice(Dictionary<Guid, decimal> products)
    {
        var discountFactor = 1.0M - Discount;

        return products[ProductId] * discountFactor;
    }

    public override List<Guid> ListRelatedProductsIds()
    {
        return [ProductId];
    }

    private PercentageDiscountOffer() : base(Guid.NewGuid())
    {

    }
}
