using Microsoft.EntityFrameworkCore;
using MiniGram.Api.Common;
using MiniGram.Api.Configuration;
using MiniGram.Api.Storage;
using MiniGram.Api.Utils;
using MiniGram.Contracts.Models;

namespace MiniGram.Api.Handlers.Auth;

public interface ILoginHandler
{
    Task<IResult> Handle(LoginRequest request, CancellationToken cancellationToken = default);
}


public class LoginHandler(MiniGramContext context, IValidator validator, IAuthService authService, IJwtService jwt, IHttpContextAccessor httpContextAccessor): ILoginHandler
{
    
    public async Task<IResult> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var validation = ValidateRequest(request);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.Errors);
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (user == null)
            return Results.NotFound(new { Message = "User not found" });
        
        // TODO: Send out verification email to user and flip this flag
        // if (!user.IsActive)
        //     return Results.BadRequest(new { Message = "User has not verified its email yet" });

        var credentials = await context.Credentials.FindAsync([user.UserId], cancellationToken);
        if (credentials == null) return Results.NotFound(new { Message = "Credentials not found" });
        
        if (authService.CheckPassword(request.Password, credentials.Password))
        {
            var accessToken = jwt.GenerateToken(user.UserId, user.Email, user.Username, user.FirstName, user.LastName);
            httpContextAccessor.HttpContext?.Response.Headers.Add("Authorization", $"Bearer {accessToken}"); // Bug in Scalar right now, doesn't work.
            httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,  // (via HTTPS)
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            return Results.Ok(new { Message = $"Successfully logged in as {user.FirstName} {user.LastName}!" });
        }
        
        return Results.BadRequest(new { Message = "Incorrect username / password" });
    }
    
    IValidator ValidateRequest(LoginRequest request)
    {
        validator.Validate(request);
        return validator;
    }
    
}