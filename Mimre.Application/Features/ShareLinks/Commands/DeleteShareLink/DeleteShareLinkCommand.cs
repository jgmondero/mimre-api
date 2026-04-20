using MediatR;

namespace Mimre.Application.Features.ShareLinks.Commands.DeleteShareLink;

public record DeleteShareLinkCommand(Guid ShareLinkId) : IRequest;
