﻿using Domain.Products;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.Update;

public record UpdateProductCommand(
    Guid Id,
    string? Name,
    string? Description,
    int? Quantity,
    decimal? PurchasePrice,
    decimal? CustomerPrice,
    string? Sku,
    bool NullSku) : IRequest<Result<Product>>;

