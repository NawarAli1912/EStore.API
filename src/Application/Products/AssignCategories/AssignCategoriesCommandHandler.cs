﻿using Application.Common.DatabaseAbstraction;
using Domain.Categories;
using Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Products.AssignCategories;

internal sealed class AssignCategoriesCommandHandler
        : IRequestHandler<AssignCategoriesCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context;

    public AssignCategoriesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Updated>> Handle(AssignCategoriesCommand request, CancellationToken cancellationToken)
    {
        var product = await _context
            .Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return DomainError.Products.NotFound;
        }

        var categoriesDict = await _context
            .Categories
            .Where(c => request.CategoriesIds.Contains(c.Id))
            .ToDictionaryAsync(i => i.Id, i => i, cancellationToken);

        List<Category> productCateogries = [];
        foreach (var categoryId in request.CategoriesIds)
        {
            if (categoriesDict.TryGetValue(categoryId, out var category))
            {
                productCateogries.Add(category);
            }
        }

        product.AssignCategories(productCateogries);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
