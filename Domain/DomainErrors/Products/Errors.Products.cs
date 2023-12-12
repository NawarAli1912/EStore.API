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

        public static Error InOrder = Error.Validation("Product.InOrder", "The product is currently included on an active order, so it can't be deleted.");

        public static Error Inactive(string name) => Error.Validation("Product.Inactive", $"The product {name}, is incative.");
    }
}
