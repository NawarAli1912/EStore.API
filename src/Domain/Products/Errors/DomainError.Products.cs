﻿using SharedKernel.Primitives;

namespace Domain.Errors;

public static partial class DomainError
{
    public static class Products
    {
        public static Error NotFound = Error.NotFound(
            "Product.NotFound",
            "The requested product doesn't exists.");

        public static Error StockError(string name) => Error.Validation(
            "Product.StockError",
            $"The requested quantity of {name} is not currently available.");

        public static Error InOrder = Error.Validation(
            "Product.InOrder",
            "The product is currently included on an active order, so it can't be deleted.");

        public static Error Deleted(string name) => Error.Validation(
            "Product.Inactive",
            $"The product {name}, is deleted.");

        public static Error OutOfStock(string name) => Error.Validation(
            "Product.Inactive",
            $"The product {name}, is out of stock.");

        public static Error NotExists(string name) => Error.Validation(
            "Product.NotExists", $"The product {name}," +
            $" doesn't exists.");

        public static Error InvalidState(string name) => Error.Validation(
            "Product.InvalidState",
            $"The product {name}, is on an invalid state.");

        public static Error UnassignedCategory(string name, Guid categoryId) =>
            Error.Validation("Product.UnassignedCategory",
                $"The product {name}, wasn't categorized by {categoryId}");

        public static Error NotPresentOnTheDictionary =>
            Error.Unexpected("Product.NotPresentOnTheDictionary",
            $"The product was not present in the dictionary, contact the development team.");
    }

    public static class Ratings
    {
        public static Error InvalidRatingValue =
            Error.Validation("Rating.InvalidRatingValue",
                "Rating value must be between 0 and 5");

        public static Error NotFound =
            Error.Validation("Rating.NotFound",
                "Rating doesn't exists.");
    }
}
