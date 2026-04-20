using MediatR;

namespace Mimre.Application.Features.Photos.Commands.DeletePhoto;

public record DeletePhotoCommand(Guid PhotoId) : IRequest;
