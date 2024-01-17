using Application.Common.Repository;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.List;

internal sealed class ListProductsQueryHandler
    : IRequestHandler<ListProductsQuery, Result<ListProductResult>>
{
    private readonly IProductsRepository _productsRepository;

    public ListProductsQueryHandler(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public async Task<Result<ListProductResult>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _productsRepository
            .ListByFilter(
                request.Filter,
                request.Page,
                request.PageSize);

        return new ListProductResult(
            result.Item1,
            result.Item2);
    }
}
