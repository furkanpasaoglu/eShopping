using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Mapster;
using MediatR;
using Shared.BuildingBlocks.CQRS;
using Shared.BuildingBlocks.Results;
using Catalog.Application.ReadModels;

namespace Catalog.Application.Commands.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductWriteRepository writeRepository,
    IProductReadRepository readRepository,
    IPublisher publisher)
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var result = Product.Create(
            request.Name,
            request.Price,
            request.Currency,
            request.Category,
            request.Stock,
            request.Description,
            request.ImageUrl);

        if (result.IsFailure)
            return result.Error;

        var product = result.Value;

        await writeRepository.AddAsync(product, cancellationToken);
        await readRepository.UpsertAsync(product.Adapt<ProductReadModel>(), cancellationToken);

        foreach (var domainEvent in product.DomainEvents)
            await publisher.Publish(domainEvent, cancellationToken);

        product.ClearDomainEvents();

        return product.Id.Value;
    }
}
