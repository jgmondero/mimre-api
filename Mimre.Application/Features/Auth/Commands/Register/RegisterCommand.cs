using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password) : IRequest<UserDto>;
