namespace Catalog.Application.DTOs;

public sealed record UpdateProductRequest(
    string Name,
    string Category,
    decimal Price,
    string Currency,
    string? Description,
    string? ImageUrl);
