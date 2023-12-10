using Domain.Kernal;

namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class Sku
    {
        public static Error InvalidLength = Error.Validation("Sku.InvalidLength", "Sku length is invalid.");
    }

    public static class Product
    {
        public static Error NotFound = Error.NotFound("Product.NotFound", "The requested product doesn't exists.");

        public static Error StockError = Error.Validation("Product.StockError", "The requested quantity is not currently available.");
    }
}
