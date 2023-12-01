using Application.Common.Data;
using Application.Common.Models;
using Domain.Kernal;
using Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Products.List;
internal sealed class ListProductsQueryHandler(IApplicationDbContext context) :
    IRequestHandler<ListProductsQuery, Result<PagedList<Product>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<PagedList<Product>>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var productsQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            productsQuery = productsQuery.Where(p =>
                    EF.Functions.Contains(p.Name, ConvertToContainsSyntax(request.SearchTerm)) ||
                    EF.Functions.Contains(p.Description, ConvertToContainsSyntax(request.SearchTerm)) ||
                    EF.Functions.FreeText(p.Name, request.SearchTerm));
        }

        if (request.SortOrder?.ToLower() == "desc")
        {
            productsQuery = productsQuery.OrderByDescending(GetSortProperty(request));
        }
        else
        {
            productsQuery = productsQuery.OrderBy(GetSortProperty(request));
        }

        return await PagedList<Product>.CreateAsync(productsQuery, request.Page, request.PageSize);
    }

    private static Expression<Func<Product, object>> GetSortProperty(ListProductsQuery request)
    {
        return request.SortColumn?.ToLower() switch
        {
            "name" => product => product.Name,
            "price" => product => product.Price.Value,
            "currency" => product => product.Price.Currency,
            _ => product => product.Id
        };
    }

    private static string ConvertToContainsSyntax(string searchTerm)
    {
        var words = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var containsTerms = words.Select(word => $"\"*{word}*\"");
        var containsQuery = string.Join($" AND ", containsTerms);
        return containsQuery;
    }

}
