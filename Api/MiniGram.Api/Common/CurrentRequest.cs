using System.Security.Claims;

namespace MiniGram.Api.Common;

public interface ICurrentRequest
{
    string Token { get; }
    Guid UserId { get; }
    string Username { get; }
    string FullName { get; }
}

public class CurrentRequest(IHttpContextAccessor httpContextBase) : ICurrentRequest
{
    public ClaimsPrincipal User => httpContextBase.HttpContext.User.Identity.IsAuthenticated
        ? httpContextBase.HttpContext.User
        : new ClaimsPrincipal();

    public string Token => httpContextBase.HttpContext?.Request.Headers.Authorization.FirstOrDefault()?.Split(" ")
                               .LastOrDefault()
                           ?? // since scalar isn't working with Authorization headers right now.. using the access_token instead.
                           httpContextBase.HttpContext?.Request.Cookies["access_token"] ?? string.Empty;

    public Guid UserId => Guid.TryParse(User?.FindFirst("UserId")?.Value, out var userId) ? userId : Guid.Empty;
    public string Username => User?.Identity?.Name ?? string.Empty;

    public string FullName =>
        $"{User?.FindFirst(ClaimTypes.GivenName)?.Value} {User?.FindFirst(ClaimTypes.Surname)?.Value}";
}
