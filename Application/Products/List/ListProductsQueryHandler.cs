using Application.Common.Data;
using Domain.Kernal;
using Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Products.List;

internal sealed class ListProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListProductsQuery, Result<ListProductResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ListProductResult>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var productsQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Filter.SearchTerm))
        {
            productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(request.Filter.SearchTerm) ||
                    EF.Functions.Contains(p.Description, ConvertToContainsSyntax(request.Filter.SearchTerm)) ||
                    EF.Functions.FreeText(p.Name, request.Filter.SearchTerm));
        }

        if (request.Filter.MinPrice is not null)
        {
            productsQuery = productsQuery
                .Where(p => p.CustomerPrice.Value > request.Filter.MinPrice);
        }

        if (request.Filter.MaxPrice is not null)
        {
            productsQuery = productsQuery
                .Where(p => p.CustomerPrice.Value < request.Filter.MaxPrice);
        }

        if (request.Filter.MinPrice is not null)
        {
            productsQuery = productsQuery
                .Where(p => p.Quantity > request.Filter.MinQuantity);
        }

        if (request.Filter.MaxPrice is not null)
        {
            productsQuery = productsQuery
                .Where(p => p.Quantity < request.Filter.MaxQuantity);
        }

        if (request.SortOrder?.ToLower() == "desc")
        {
            productsQuery = productsQuery
                            .OrderByDescending(GetSortProperty(request));
        }
        else
        {
            productsQuery = productsQuery
                            .OrderBy(GetSortProperty(request));
        }

        var result = await productsQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new ListProductResult(
            result,
            await productsQuery
                .CountAsync(cancellationToken));
    }

    private static Expression<Func<Product, object>> GetSortProperty(ListProductsQuery request)
    {
        return request.SortColumn?.ToLower() switch
        {
            "name" => product => product.Name,
            "price" => product => product.CustomerPrice.Value,
            "currency" => product => product.CustomerPrice.Currency,
            _ => product => product.Id
        };
    }

    private static string ConvertToContainsSyntax(string searchTerm)
    {
        var words = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var containsTerms = words.Select(word => $"\"*{word}*\"");
        var containsQuery = string.Join($" OR ", containsTerms);
        return containsQuery;
    }
}
