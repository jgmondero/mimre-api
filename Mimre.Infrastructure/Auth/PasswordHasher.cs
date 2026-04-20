using Microsoft.AspNetCore.Identity;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Infrastructure.Auth;

// ASP.NET Core's built-in PBKDF2 hasher
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string password, string hash) =>
        _hasher.VerifyHashedPassword(null!, hash, password)
            != PasswordVerificationResult.Failed;
}
