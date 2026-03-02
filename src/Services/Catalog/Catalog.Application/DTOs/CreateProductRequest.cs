namespace Catalog.Application.DTOs;

public sealed record CreateProductRequest(
    string Name,
    string Category,
    decimal Price,
    string Currency,
    int Stock,
    string? Description,
    string? ImageUrl);
