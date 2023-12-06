namespace Contracts.Products;
public record UpdateProductBasicInfoRequest(
    string? Name,
    string? Description
    );
