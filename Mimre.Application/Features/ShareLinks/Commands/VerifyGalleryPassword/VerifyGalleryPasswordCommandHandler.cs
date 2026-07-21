using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Mimre.Application.Features.ShareLinks.Commands.VerifyGalleryPassword;

public class VerifyGalleryPasswordCommandHandler(
    IUnitOfWork uow,
    IPasswordHasher passwordHasher,
    ILogger<VerifyGalleryPasswordCommandHandler> logger)
    : IRequestHandler<VerifyGalleryPasswordCommand, bool>
{
    public async Task<bool> Handle(VerifyGalleryPasswordCommand request, CancellationToken ct)
    {
        var link = await uow.ShareLinks.GetByTokenAsync(request.Token, ct)
            ?? throw new NotFoundException(nameof(ShareLink), request.Token);

        if (link.IsExpired())
            throw new DomainException("This share link has expired.");

        if (!link.Gallery.IsPublished)
            throw new DomainException("This gallery is not currently available.");

        // No password set — no verification needed
        if (link.Gallery.PasswordHash is null)
            return true;

        var isValid = passwordHasher.Verify(request.Password, link.Gallery.PasswordHash);

        logger.LogInformation("Gallery password verification {Result}. {Token}",
            isValid ? "succeeded" : "failed", request.Token);

        return isValid;
    }
}
