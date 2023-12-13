using Domain.Kernal;

namespace Domain.DomainErrors;
public static partial class Errors
{
    public static class Orders
    {
        public static Error NotFound =
            Error.NotFound("Order.NotFound", "The order doesn't exists.");
    }
}
