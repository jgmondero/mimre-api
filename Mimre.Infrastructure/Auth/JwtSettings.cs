namespace Mimre.Infrastructure.Auth;

public class JwtSettings
{
    public string SecretKey { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int AccessTokenExpiryMinutes { get; init; } = 15;
}
