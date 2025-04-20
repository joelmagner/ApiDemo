using Microsoft.EntityFrameworkCore;
using MiniGram.Api.Common;
using MiniGram.Api.Storage;
using MiniGram.Api.Storage.Entities;
using MiniGram.Api.Utils;
using MiniGram.Contracts.Models;

namespace MiniGram.Api.Handlers;

public interface ICreateUserHandler
{
    Task<IResult> Handle(CreateUserRequest request, CancellationToken cancellationToken = default);
}

public class CreateUserHandler(
    MiniGramContext context,
    IAuthService auth,
    IValidator validate) : ICreateUserHandler
{
    public async Task<IResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
     
        var validation = ValidateRequest(request);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.Errors);

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Username == request.UserName || u.Email == request.Email, cancellationToken);

        if (existingUser != null)
        {
            if (existingUser.Username == request.UserName)
                return Results.Conflict(new { Message = "User already exists." });

            if (existingUser.Email == request.Email)
                return Results.Conflict(new { Message = "Email already exists." });
        }

        var userId = Guid.NewGuid();
        var user = new User
        {
            UserId = userId,
            Username = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName.ToPascalCase(),
            LastName = request.LastName.ToPascalCase(),
            Created = DateTime.UtcNow,
            Updated = null
        };
        
        var credentials = new Credentials
        {
            UserId = userId,
            Password = auth.HashPassword(request.Password),
            Created = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.Credentials.Add(credentials);
        await context.SaveChangesAsync(cancellationToken);

        return Results.Created($"/users/{user.UserId}", user);
    }

    IValidator ValidateRequest(CreateUserRequest request)
    {
        validate.Validate(request);
        validate.Check(request.UserName, nameof(request.UserName), $"{nameof(request.UserName)} is required.");
        validate.Check(request.FirstName, nameof(request.FirstName), $"{nameof(request.FirstName)} must be at least 2 characters.", s => s.Length >= 2);
        validate.Check(request.LastName, nameof(request.LastName), $"{nameof(request.LastName)} must be at least 2 characters.", s => s.Length >= 2);
        validate.Check(request.Password, nameof(request.Password), $"{nameof(request.Password)} must be at least 6 characters.", s => s.Length >= 6);
        validate.Check(request.Email, nameof(request.Email), "Email must be valid.", s => s.Contains('@'));
        return validate;
    }
    
   
}