using Domain.Kernal;

namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class Category
    {
        public static Error NotFound = Error.NotFound("Category.NotFound", "The requested category doesn't exists.");
    }
}
