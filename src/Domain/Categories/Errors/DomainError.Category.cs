using SharedKernel;

namespace Domain.Categories.Errors;

public static partial class DomainError
{
    public static class Category
    {
        public static Error NotFound = Error.NotFound("Category.NotFound", "The requested category doesn't exists.");
    }
}
