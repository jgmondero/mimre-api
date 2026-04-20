using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, ITokenService tokenService)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await uow.Users.GetByEmailAsync(request.Email, ct)
            ?? throw new DomainException("Invalid email or password.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid email or password.");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        var userDto = new UserDto(user.Id, user.Email, user.FullName, user.PlanTier, user.StorageUsedBytes);
        return new LoginResult(accessToken, refreshToken, userDto);
    }
}
