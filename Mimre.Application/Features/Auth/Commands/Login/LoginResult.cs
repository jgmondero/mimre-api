using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Auth.Commands.Login;

public record LoginResult(string AccessToken, string RefreshToken, UserDto User);
