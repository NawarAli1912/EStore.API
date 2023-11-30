﻿using Domain.Kernal;
using Domain.Products;
using MediatR;

namespace Application.Products.Create;

public record CreateProductCommand(
    string Name,
    string Description,
    int Quantity,
    decimal Price,
    string Currency,
    string? Sku) : IRequest<Result<Product>>;