using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.ShareLinks.Commands.DeleteShareLink;

public record DeleteShareLinkCommand(Guid ShareLinkId, [property: JsonIgnore] Guid UserId = default) : IRequest;
