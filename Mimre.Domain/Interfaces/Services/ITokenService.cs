using Mimre.Domain.Entities;

namespace Mimre.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
