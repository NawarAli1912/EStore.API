using FluentValidation;

namespace Application.Products.List;
public sealed class ListProductQueryValidation : AbstractValidator<ListProductsQuery>
{
    private readonly List<string> _validSortOrders = ["asc", "desc"];
    private readonly List<string> _validSrotColumns = ["name", "price", "quantity"];
    public ListProductQueryValidation()
    {
        When(query => query.Filter.SortOrder is not null, () =>
        {
            RuleFor(query => query.Filter.SortOrder)
                .NotEmpty()
                .Must(value => _validSortOrders.Contains(value!.ToLower()))
                .WithMessage("Sort order must be either asc or desc.");
        });

        When(query => query.Filter.SortColumn is not null, () =>
        {
            RuleFor(query => query.Filter.SortColumn)
                .NotEmpty()
                .Must(value => _validSrotColumns.Contains(value!.ToLower()))
                .WithMessage("Sort column must be one of [name, price, quantity]");
        });

    }
}
