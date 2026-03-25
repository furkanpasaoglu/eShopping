using FluentValidation;

namespace Payment.Application.Commands.ReservePayment;

internal sealed class ReservePaymentCommandValidator : AbstractValidator<ReservePaymentCommand>
{
    public ReservePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
    }
}
