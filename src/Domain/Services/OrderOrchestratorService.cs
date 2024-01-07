using Domain.Customers;
using Domain.Errors;
using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Services.OfferAdditionStrategy;
using Domain.Services.OfferRemovalStrategy;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Services;
public class OrderOrchestratorService(
    Dictionary<Guid, Product> productDict,
    Dictionary<Guid, Offer>? offersDict = default)
{
    private readonly Dictionary<Guid, Product> ProductDict = productDict;
    private readonly Dictionary<Guid, Offer> OffersDict = offersDict ?? [];

    /// <summary>
    /// Processes a new order for a specified customer.
    /// This function updates the product quantities based on the customer's cart,
    /// integrates shipping information, and handles potential errors.
    /// It starts by validating the cart contents,
    /// then creates an order using customer and shipping details.
    /// The function assesses product availability,
    /// calculates prices (including for offers),
    /// and adds these items to the order.
    /// If errors arise during this process, such as with product availability,
    /// the function returns these errors. On successful completion,
    /// the customer's cart is cleared, and the order is returned.
    /// </summary>
    /// <param name="customer">
    ///     The customer for whom the order is being created.
    /// </param>
    /// <param name="productDict">
    ///     Dictionary mapping each product's ID to its corresponding product details.
    /// </param>
    /// <param name="offersDict">
    ///     Dictionary holding offer information, keyed by offer ID.
    /// </param>
    /// <param name="shippingCompany">
    ///     The shipping company to handle the order delivery.
    /// </param>
    /// <param name="shippingCompanyAddress">
    ///     The address of the shipping company.
    /// </param>
    /// <param name="phoneNumber">
    ///     Contact phone number for the order.
    /// </param>
    /// <returns>
    ///     A Result object containing the created order if the process is successful. In case of errors (e.g., empty cart, unavailable products), it returns a list of these errors.
    /// </returns>
    public Result<Order> CreateOrder(
        Customer customer,
        ShippingCompany shippingCompany,
        string shippingCompanyAddress,
        string phoneNumber)
    {
        if (offersDict is null)
        {
            return DomainError.Offers.UnintializedOffersDict;
        }

        var cartItems = customer
            .Cart
            .CartItems
            .ToList();

        if (customer.Cart.CartItems.Count == 0)
        {
            return DomainError.Carts.EmptyCart;
        }

        var order = Order.Create(
            customer.Id,
            shippingCompany,
            shippingCompanyAddress,
            phoneNumber);

        var errors = ValidateProductsStatus(productDict);
        if (errors.Count > 0)
        {
            return errors;
        }

        foreach (var cartItem in cartItems)
        {
            if (cartItem.Type == ItemType.Product)
            {
                var product = productDict[cartItem.ItemId];

                var addItemResult = order.AddItems(
                    product.Id,
                    product.CustomerPrice,
                    cartItem.Quantity);

                if (addItemResult.IsError)
                {
                    errors.AddRange(addItemResult.Errors);
                }

                continue;
            }

            var offer = offersDict[cartItem.ItemId];
            var offerAdditionStrategy = OfferAdditionStrategyFactory
                .GetStrategy(offer, productDict);

            offerAdditionStrategy.Handle(order, cartItem.Quantity);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        customer.ClearCart();

        return order;
    }

    /// <summary>
    /// Approves an order, decreasing product quantities and updating order status if the order is not rejected.
    /// </summary>
    /// <param name="order">
    ///     The order to be approved.
    /// </param>
    /// <param name="productDict">
    ///     A dictionary containing product information (mapping product ID to product).
    /// </param>
    /// <returns>
    ///     A Result indicating whether the approval was successful (Result.Updated).
    ///     If errors occur during approval, the Result will contain a list of error details.
    /// </returns>
    public Result<Updated> Approve(Order order)
    {
        if (order.LineItems is null)
        {
            return DomainError.Orders.EmptyLineItems;
        }

        var errors = ValidateProductsStatus(productDict);
        if (errors.Count > 0)
        {
            return errors;
        }

        var lineItemsGroups = order
            .LineItems
            .GroupBy(li => li.ProductId);

        foreach (var group in lineItemsGroups)
        {
            var product = productDict[group.Key];

            var decreaseQuantityResult = product.DecreaseQuantity(group.Count());

            if (decreaseQuantityResult.IsError)
            {
                errors.AddRange(decreaseQuantityResult.Errors);
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        order.Approve();

        return Result.Updated;
    }

    public Result<Updated> UpdateOffersItems(
        Order order,
        Dictionary<Guid, int> offerToAdd,
        Dictionary<Guid, int> offersToDelete)
    {
        List<Error> errors = [];
        foreach (var items in offerToAdd)
        {
            var offerAdditionStrategy =
                OfferAdditionStrategyFactory.GetStrategy(OffersDict[items.Key], ProductDict);

            var additionResult = offerAdditionStrategy.Handle(order, items.Value);

            if (additionResult.IsError)
            {
                errors.AddRange(additionResult.Errors);
            }
        }

        foreach (var item in offersToDelete)
        {
            var offerRemovalStrategy =
                OfferRemovalStrategyFactory.GetStrategy(OffersDict[item.Key]);

            var removalResult = offerRemovalStrategy.Handle(order, item.Value);

            if (removalResult.IsError)
            {
                errors.AddRange(removalResult.Errors);
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return Result.Updated;
    }

    /// <summary>
    /// Updates the items in the order based on the provided dictionaries.
    /// </summary>
    /// <param name="order">
    ///     The order to be updated.
    /// </param>
    /// <param name="productIdToProduct">
    ///     A dictionary containing product information (mapping product ID to product).
    /// </param>
    /// <param name="itemsToAddQuantities">
    ///     A dictionary specifying the quantities of items to add (mapping product ID to quantity to be added).
    /// </param>
    /// <param name="itemsToDeleteQuantities">
    ///     A dictionary specifying the quantities of items to delete (mapping product ID to quantity to be deleted).
    /// </param>
    /// <returns>
    ///     A result indicating whether the update was successful (Result.Updated) or any errors that occurred during the update.
    ///     If errors occur, the result will contain a list of error details.
    /// </returns>
    public Result<Updated> UpdateProductItems(
        Order order,
        Dictionary<Guid, int> itemsToAddQuantities,
        Dictionary<Guid, int> itemsToDeleteQuantities)
    {

        List<Error> errors = [];
        foreach (var productId in itemsToAddQuantities.Keys)
        {

            if (!ProductDict.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Products.NotFound);
                continue;
            }

            var productValidation = ValidateProductStatus(product);
            if (productValidation.IsError)
            {
                errors.AddRange(productValidation.Errors);
                continue;
            }

            var decreaseResult = product.DecreaseQuantity(itemsToAddQuantities[productId]);
            if (decreaseResult.IsError)
            {
                errors.Add(DomainError.Products.StockError(product.Name));
                continue;
            }

            order.AddItems(
                product.Id,
                product.CustomerPrice,
                itemsToAddQuantities[productId]);
        }

        foreach (var productId in itemsToDeleteQuantities.Keys)
        {
            if (!ProductDict.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Products.NotFound);
                continue;
            }

            product.IncreaseQuantity(itemsToDeleteQuantities[productId]);
            var removalResult = order.RemoveItems(product.Id, itemsToDeleteQuantities[productId]);
            if (removalResult.IsError)
            {
                errors.AddRange(removalResult.Errors);
            }

        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return Result.Updated;
    }

    private List<Error> ValidateProductsStatus(Dictionary<Guid, Product> productDict)
    {
        var errors = new List<Error>();
        foreach (var product in productDict.Values)
        {
            if (product.Status != ProductStatus.Active)
            {
                var result = ValidateProductStatus(product);

                if (result.IsError)
                    errors.AddRange(result.Errors);
            }
        }

        return errors;
    }

    private Result<Success> ValidateProductStatus(Product product)
    {

        return product.Status != ProductStatus.Active ?
            product.Status switch
            {
                ProductStatus.Deleted =>
                    DomainError.Products.Deleted(product.Name),
                ProductStatus.OutOfStock =>
                    DomainError.Products.OutOfStock(product.Name),
                _ =>
                    DomainError.Products.InvalidState(product.Name)
            } : Result.Success;
    }
}
