using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.Common.Settings;
using Mimre.Application.DTOs;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;
using RefreshTokenEntity = Mimre.Domain.Entities.RefreshToken;

namespace Mimre.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, ITokenService tokenService, IOptions<JwtSettings> jwtSettings, ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await uow.Users.GetByEmailAsync(request.Email, ct)
            ?? throw new DomainException("Invalid email or password.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid email or password.");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshTokenValue = tokenService.GenerateRefreshToken();

        var refreshToken = RefreshTokenEntity.Create(
            user.Id,
            refreshTokenValue,
            DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpiryDays));

        uow.RefreshTokens.Add(refreshToken);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("User logged in. {UserId} {Email}", user.Id, user.Email);
        var userDto = new UserDto(user.Id, user.Email, user.FullName, user.PlanTier, user.StorageUsedBytes);
        return new LoginResult(accessToken, refreshTokenValue, userDto);
    }
}
