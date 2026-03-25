namespace Payment.Application.Abstractions;

public interface IPaymentRepository
{
    Task<Payment.Domain.Entities.Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Payment.Domain.Entities.Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task AddAsync(Payment.Domain.Entities.Payment payment, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
