﻿using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Products.Get;

internal sealed class GetProductQueryHandler
    : IRequestHandler<GetProductQuery, Result<Product>>
{
    private readonly IApplicationDbContext _context;

    public GetProductQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Product>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {

        var product = await _context
            .Products
            .Include(p => p.Categories)
            .ThenInclude(c => c.SubCategories)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            return DomainError.Products.NotFound;
        }
        return product;
    }
}
