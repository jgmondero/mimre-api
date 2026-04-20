using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Common;

namespace Mimre.Application.Common.Behaviors;

// Dispatches domain events raised on entities after SaveChanges
public sealed class DomainEventBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork, IPublisher publisher)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var response = await next(ct);

        // Collect all domain events from tracked entities after handler runs
        var domainEvents = unitOfWork.GetTrackedEntities()
            .OfType<BaseEntity>()
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var entity in unitOfWork.GetTrackedEntities().OfType<BaseEntity>())
            entity.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        return response;
    }
}
