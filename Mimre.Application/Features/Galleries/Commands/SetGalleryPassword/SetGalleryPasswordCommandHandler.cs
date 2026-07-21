using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Mimre.Application.Features.Galleries.Commands.SetGalleryPassword;

public class SetGalleryPasswordCommandHandler(
    IUnitOfWork uow,
    IPasswordHasher passwordHasher,
    ILogger<SetGalleryPasswordCommandHandler> logger)
    : IRequestHandler<SetGalleryPasswordCommand>
{
    public async Task Handle(SetGalleryPasswordCommand request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        if (gallery.UserId != request.UserId)
            throw new DomainException("Access denied.");

        // null password removes the password protection
        var passwordHash = request.Password is not null
            ? passwordHasher.Hash(request.Password)
            : null;

        gallery.SetPassword(passwordHash);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Gallery password {Action}. {GalleryId}",
            request.Password is not null ? "set" : "removed",
            request.GalleryId);
    }
}
