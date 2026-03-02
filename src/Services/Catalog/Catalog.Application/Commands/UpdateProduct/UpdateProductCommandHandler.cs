using Catalog.Application.Abstractions;
using Catalog.Application.ReadModels;
using Catalog.Domain.Errors;
using Catalog.Domain.ValueObjects;
using Mapster;
using MediatR;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Results;

namespace Catalog.Application.Commands.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductWriteRepository writeRepository,
    IProductReadRepository readRepository,
    IPublisher publisher)
    : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await writeRepository.GetByIdAsync(
            ProductId.From(request.Id), cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        var updateResult = product.UpdateDetails(
            request.Name,
            request.Price,
            request.Currency,
            request.Category,
            request.Description,
            request.ImageUrl);

        if (updateResult.IsFailure)
            return updateResult.Error;

        await writeRepository.UpdateAsync(product, cancellationToken);
        await readRepository.UpsertAsync(product.Adapt<ProductReadModel>(), cancellationToken);

        foreach (var domainEvent in product.DomainEvents)
            await publisher.Publish(domainEvent, cancellationToken);

        product.ClearDomainEvents();

        return Result.Success();
    }
}
