namespace Catalog.Application.DTOs;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    string Currency,
    int Stock,
    string? Description,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
