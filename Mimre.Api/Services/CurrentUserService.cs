using Mimre.Api.Extensions;

namespace Mimre.Api.Services;

public class CurrentUserService(IHttpContextAccessor accessor)
{
    public Guid UserId => accessor.HttpContext!.User.GetUserId();
}
