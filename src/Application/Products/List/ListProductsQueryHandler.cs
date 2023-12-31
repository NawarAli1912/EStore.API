﻿using Application.Common.Repository;
using Domain.Products;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.List;

internal sealed class ListProductsQueryHandler(IProductsRepository productsRepository)
    : IRequestHandler<ListProductsQuery, Result<ListProductResult>>
{
    private readonly IProductsRepository _productsRepository = productsRepository;

    public async Task<Result<ListProductResult>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _productsRepository
            .ListByFilter(
                request.Filter,
                request.Page,
                request.PageSize);


        result.Item1 = SortResult(result.Item1, request.SortColumn, request.SortOrder);

        return new ListProductResult(
            result.Item1,
            result.Item2);
    }

    private List<Product> SortResult(
        List<Product> products,
        string? sortColumn = "",
        string? sortOrder = "asc")
    {
        return sortColumn?.ToLowerInvariant() switch
        {
            "name" => sortOrder == "desc" ? [.. products.OrderByDescending(p => p.Name)] :
                        [.. products.OrderBy(p => p.Name)],
            "price" => sortOrder == "desc" ? [.. products.OrderByDescending(p => p.CustomerPrice)] :
                        [.. products.OrderBy(p => p.CustomerPrice)],
            "quantity" => sortOrder == "desc" ? [.. products.OrderByDescending(p => p.Quantity)] :
                        [.. products.OrderBy(p => p.Quantity)],
            _ => products
        };
    }
}
