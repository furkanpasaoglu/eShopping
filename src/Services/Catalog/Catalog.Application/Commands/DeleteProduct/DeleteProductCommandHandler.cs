using Catalog.Application.Abstractions;
using Catalog.Domain.Errors;
using Catalog.Domain.ValueObjects;
using MediatR;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Results;

namespace Catalog.Application.Commands.DeleteProduct;

internal sealed class DeleteProductCommandHandler(
    IProductWriteRepository writeRepository,
    IProductReadRepository readRepository,
    IPublisher publisher)
    : ICommandHandler<DeleteProductCommand>
{
    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await writeRepository.GetByIdAsync(
            ProductId.From(request.Id), cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        var deleteResult = product.Delete();
        if (deleteResult.IsFailure)
            return deleteResult.Error;

        await writeRepository.UpdateAsync(product, cancellationToken);
        await readRepository.MarkDeletedAsync(product.Id.Value, cancellationToken);

        foreach (var domainEvent in product.DomainEvents)
            await publisher.Publish(domainEvent, cancellationToken);

        product.ClearDomainEvents();

        return Result.Success();
    }
}
