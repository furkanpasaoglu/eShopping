using Shared.BuildingBlocks.Results;

namespace Basket.Domain.Errors;

public static class BasketErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Basket.NotFound", "Basket was not found.");

    public static readonly Error ItemNotFound =
        Error.NotFound("Basket.ItemNotFound", "Item was not found in basket.");

    public static readonly Error InvalidQuantity =
        Error.Validation("Basket.InvalidQuantity", "Quantity must be greater than zero.");

    public static readonly Error ProductNotFound =
        Error.NotFound("Basket.ProductNotFound", "Product does not exist in the catalog.");

    public static readonly Error CatalogUnavailable =
        Error.Failure("Basket.CatalogUnavailable",
            "Unable to validate product — catalog service is unreachable.");
}
