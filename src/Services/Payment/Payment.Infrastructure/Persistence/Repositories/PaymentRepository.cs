using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Persistence.Repositories;

internal sealed class PaymentRepository(PaymentDbContext dbContext) : IPaymentRepository
{
    public Task<Payment.Domain.Entities.Payment?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var paymentId = PaymentId.From(id);
        return dbContext.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId, ct);
    }

    public async Task<IReadOnlyList<Payment.Domain.Entities.Payment>> GetByOrderIdAsync(
        Guid orderId, CancellationToken ct = default)
    {
        return await dbContext.Payments
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.ReservedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Payment.Domain.Entities.Payment payment, CancellationToken ct = default) =>
        await dbContext.Payments.AddAsync(payment, ct);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        dbContext.SaveChangesAsync(ct);
}
