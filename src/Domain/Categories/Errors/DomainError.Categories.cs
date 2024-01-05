using SharedKernel.Primitives;

namespace Domain.Errors;

public static partial class DomainError
{
    public static class Categories
    {
        public static Error NotFound = Error.NotFound("Category.NotFound", "The requested category doesn't exists.");
    }
}
