using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Mimre.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(IUnitOfWork uow, ILogger<UpdateProfileCommandHandler> logger) : IRequestHandler<UpdateProfileCommand>
{
    public async Task Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        user.UpdateProfile(request.FullName);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("User profile updated. {UserId}", request.UserId);
    }
}
