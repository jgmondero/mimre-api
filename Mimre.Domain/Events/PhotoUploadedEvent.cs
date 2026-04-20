using Mimre.Domain.Common;

namespace Mimre.Domain.Events;

public record PhotoUploadedEvent(Guid PhotoId, Guid AlbumId) : IDomainEvent;
