﻿using Application.Common.Data;
using Domain.Categories;
using Domain.DomainErrors;
using Domain.Kernal;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Categories.Create;

internal class CreateCategoryCommandHandler(IApplicationDbContext context)
        : IRequestHandler<CreateCategoryCommand, Result<Created>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Created>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {

        Category? parentCategory = default!;
        var products = await _context
                        .Products
                        .Where(p => request.Products.Contains(p.Id))
                        .ToListAsync(cancellationToken);

        if (request.ParentCategoryId is not null)
        {
            parentCategory = await _context
                    .Categories
                    .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId, cancellationToken: cancellationToken);

            if (parentCategory is null)
            {
                return Errors.Category.NotFound;
            }
        }

        var categroy = Category.Create(
            Guid.NewGuid(),
            request.Name,
            parentCategory);

        categroy.AssignProducts(products);

        await _context.Categories.AddAsync(categroy, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Created;
    }
}