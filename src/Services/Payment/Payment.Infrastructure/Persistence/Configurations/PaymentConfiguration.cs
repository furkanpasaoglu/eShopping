using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Enums;
using Payment.Domain.ValueObjects;

namespace Payment.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment.Domain.Entities.Payment>
{
    public void Configure(EntityTypeBuilder<Payment.Domain.Entities.Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => PaymentId.From(value))
            .HasColumnName("id");

        builder.Property(p => p.OrderId)
            .IsRequired()
            .HasColumnName("order_id");

        builder.Property(p => p.CustomerId)
            .IsRequired()
            .HasColumnName("customer_id");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion(s => (int)s, v => (PaymentStatus)v)
            .HasColumnName("status");

        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("amount").IsRequired().HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("currency").IsRequired().HasMaxLength(3);
        });

        builder.OwnsOne(p => p.Card, card =>
        {
            card.Property(c => c.MaskedNumber).HasColumnName("card_masked_number").IsRequired().HasMaxLength(20);
            card.Property(c => c.ExpiryMonth).HasColumnName("card_expiry_month").IsRequired().HasMaxLength(2);
            card.Property(c => c.ExpiryYear).HasColumnName("card_expiry_year").IsRequired().HasMaxLength(4);
            card.Property(c => c.CardHolderName).HasColumnName("card_holder_name").IsRequired().HasMaxLength(200);
        });

        builder.Property(p => p.TransactionId)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("transaction_id");

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500)
            .HasColumnName("failure_reason");

        builder.Property(p => p.ReservedAt)
            .IsRequired()
            .HasColumnName("reserved_at");

        builder.Property(p => p.CapturedAt)
            .HasColumnName("captured_at");

        builder.Property(p => p.RefundedAt)
            .HasColumnName("refunded_at");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("ix_payments_order_id");

        builder.HasIndex(p => p.CustomerId)
            .HasDatabaseName("ix_payments_customer_id");

        builder.Ignore(p => p.DomainEvents);
    }
}
