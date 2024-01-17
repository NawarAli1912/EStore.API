namespace Application.Common;

public static class CacheKeys
{
    public const string OffersCacheKey = "offers-key";
    public static string ProductCacheKey(Guid id) => $"product-{id}";
}
