using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;
