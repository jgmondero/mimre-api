using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string FullName,
    [property: JsonIgnore] Guid UserId = default) : IRequest;
