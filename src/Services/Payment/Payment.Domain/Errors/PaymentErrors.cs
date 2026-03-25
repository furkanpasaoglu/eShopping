using Shared.BuildingBlocks.Results;

namespace Payment.Domain.Errors;

public static class PaymentErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Payment.NotFound", "The payment was not found.");

    public static readonly Error InvalidAmount =
        Error.Validation("Payment.InvalidAmount", "Payment amount must be greater than zero.");

    public static readonly Error AlreadyReserved =
        Error.Conflict("Payment.AlreadyReserved", "The payment has already been reserved.");

    public static readonly Error AlreadyCaptured =
        Error.Conflict("Payment.AlreadyCaptured", "The payment has already been captured.");

    public static readonly Error AlreadyRefunded =
        Error.Conflict("Payment.AlreadyRefunded", "The payment has already been refunded.");

    public static readonly Error AlreadyFailed =
        Error.Conflict("Payment.AlreadyFailed", "The payment has already failed.");

    public static readonly Error CannotCaptureUnreserved =
        Error.Conflict("Payment.CannotCaptureUnreserved", "Only a reserved payment can be captured.");

    public static readonly Error CannotRefundUncaptured =
        Error.Conflict("Payment.CannotRefundUncaptured", "Only a captured payment can be refunded.");
}
