using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        return new UserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.PlanTier,
            user.StorageUsedBytes);
    }
}
