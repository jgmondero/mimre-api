using MediatR;
using Microsoft.Extensions.Options;
using Mimre.Application.Common.Interfaces;
using RefreshTokenEntity = Mimre.Domain.Entities.RefreshToken;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Settings;

namespace Mimre.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUnitOfWork uow,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtSettings,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var existing = await uow.RefreshTokens.GetByTokenAsync(request.RefreshToken, ct)
            ?? throw new DomainException("Invalid refresh token.");

        if (!existing.IsActive())
            throw new DomainException("Refresh token has expired or been revoked.");

        // Revoke old token
        existing.Revoke();

        // Issue new tokens
        var newAccessToken = tokenService.GenerateAccessToken(existing.User);
        var newRefreshTokenValue = tokenService.GenerateRefreshToken();

        var newRefreshToken = RefreshTokenEntity.Create(
            existing.UserId,
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpiryDays));

        uow.RefreshTokens.Add(newRefreshToken);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Token refreshed. {UserId}", existing.UserId);

        return new RefreshTokenResult(newAccessToken, newRefreshTokenValue);
    }
}
