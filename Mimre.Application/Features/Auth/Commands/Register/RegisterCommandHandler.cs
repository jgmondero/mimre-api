using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand, UserDto>
{
    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await uow.Users.EmailExistsAsync(request.Email, ct))
            throw new DomainException("An account with this email already exists.");

        var hash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, hash, request.FullName);

        uow.Users.Add(user);
        await uow.SaveChangesAsync(ct);

        return new UserDto(user.Id, user.Email, user.FullName, user.PlanTier, user.StorageUsedBytes);
    }
}
