namespace Shared.Contracts.Seeding;

/// <summary>
/// Deterministic seed product definitions shared across services.
/// Both Catalog and Stock seeders reference these fixed IDs
/// to ensure consistency during initial data population.
/// </summary>
public static class SeedProductCatalog
{
    public static IReadOnlyList<SeedProduct> Products { get; } =
    [
        new("d0e1a2b3-c4d5-4e6f-8a9b-0c1d2e3f4a5b", "Laptop Pro 15", "Electronics", 1299.99m, "USD", 50, "High-performance laptop with 15\" display", null),
        new("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d", "Wireless Mouse", "Electronics", 29.99m, "USD", 200, "Ergonomic wireless mouse with long battery life", null),
        new("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e", "Mechanical Keyboard", "Electronics", 89.99m, "USD", 150, "Tactile mechanical keyboard with RGB backlight", null),
        new("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f", "USB-C Hub", "Electronics", 49.99m, "USD", 300, "7-in-1 USB-C hub with HDMI and USB 3.0 ports", null),
        new("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f80", "Running Shoes", "Footwear", 119.99m, "USD", 80, "Lightweight running shoes with cushioned sole", null),
        new("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8091", "Yoga Mat", "Sports", 35.99m, "USD", 120, "Non-slip 6mm yoga mat with carrying strap", null),
        new("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8091a2", "Water Bottle", "Sports", 24.99m, "USD", 500, "1L stainless steel insulated water bottle", null),
        new("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8091a2b3", "Backpack 30L", "Accessories", 79.99m, "USD", 60, "Durable 30L backpack with laptop compartment", null),
        new("b8c9d0e1-f2a3-4b4c-5d6e-7f8091a2b3c4", "Desk Lamp", "Home", 39.99m, "USD", 90, "LED desk lamp with adjustable brightness and color temp", null),
        new("c9d0e1f2-a3b4-4c5d-6e7f-8091a2b3c4d5", "Notebook Set", "Stationery", 14.99m, "USD", 400, "Set of 3 ruled notebooks, A5 size", null),
    ];
}

public sealed record SeedProduct(
    string Id,
    string Name,
    string Category,
    decimal Price,
    string Currency,
    int Stock,
    string Description,
    string? ImageUrl)
{
    public Guid ProductId => Guid.Parse(Id);
}
