﻿using Application.Common.Data;
using Domain.Kernal;
using Domain.Products;
using MediatR;

namespace Application.Products.Create;

public sealed class CreateProductCommandHandler(IApplicationDbContext context) :
    IRequestHandler<CreateProductCommand, Result<Product>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Product>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productResult = Product.Create(
            request.Name,
            request.Description,
            request.Quantity,
            request.Price,
            request.Currency,
            request.Sku);

        if (productResult.IsError)
        {
            return productResult.Errors;
        }
        var product = productResult.Value;

        await _context.Products.AddAsync(product, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return product;
    }
}