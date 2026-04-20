using MediatR;

namespace Mimre.Application.Features.Photos.Commands.ProcessPhoto;

public record ProcessPhotoCommand(Guid PhotoId) : IRequest;
