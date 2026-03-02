using FluentValidation;

namespace Catalog.Application.Commands.AdjustStock;

internal sealed class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
