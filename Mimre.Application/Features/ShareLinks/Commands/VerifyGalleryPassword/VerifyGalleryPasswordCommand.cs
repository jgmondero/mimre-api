using MediatR;

namespace Mimre.Application.Features.ShareLinks.Commands.VerifyGalleryPassword;

public record VerifyGalleryPasswordCommand(
    string Token,
    string Password) : IRequest<bool>;
