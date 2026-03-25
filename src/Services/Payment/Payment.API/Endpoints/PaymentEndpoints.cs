namespace Payment.API.Endpoints;

internal static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this RouteGroupBuilder group)
    {
        ReservePaymentEndpoint.Map(group);
        CapturePaymentEndpoint.Map(group);
        RefundPaymentEndpoint.Map(group);
        GetPaymentByIdEndpoint.Map(group);
        GetPaymentsByOrderEndpoint.Map(group);

        return group;
    }
}
