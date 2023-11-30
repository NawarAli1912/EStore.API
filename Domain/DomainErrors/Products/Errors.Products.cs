using Domain.Kernal;

namespace Domain.DomainErrors.Products;

public static partial class Errors
{
    public static class Sku
    {
        public static Error InvalidLength = Error.Validation("Sku.InvalidLength", "Sku length is invalid.");
    }

    public static class Product
    {
        public static Error NotFound = Error.NotFound("Product.NotFound", "The requested product doesn't exists.");
    }
}
