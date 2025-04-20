using MiniGram.Contracts.Models;

namespace MiniGram.Api.Handlers.Auth;

public interface ILogoutHandler
{
    Task<IResult> Handle(LogoutRequest request, CancellationToken cancellationToken = default);
}

public class LogoutHandler(IHttpContextAccessor httpContextAccessor): ILogoutHandler
{
    
    public async Task<IResult> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
// kika här på hur jag får in CurrentRequest, använder sig av httpContextAccessor!
        httpContextAccessor.HttpContext?.Response.Cookies.Delete("access_token");
        // Return a result
        return Results.Ok(new { Message = "Logout successful" });
    }
}
