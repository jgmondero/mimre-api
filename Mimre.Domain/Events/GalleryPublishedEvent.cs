using Mimre.Domain.Common;

namespace Mimre.Domain.Events;

public record GalleryPublishedEvent(Guid GalleryId) : IDomainEvent;
