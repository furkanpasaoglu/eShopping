using FluentValidation;

namespace Basket.Application.Commands.UpsertBasketItem;

internal sealed class UpsertBasketItemCommandValidator : AbstractValidator<UpsertBasketItemCommand>
{
    public UpsertBasketItemCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
