using MiniGram.Api.Common;
using MiniGram.Api.Storage;

namespace MiniGram.Api.Handlers.Auth;

public interface IMeHandler
{
    Task<IResult> Handle(CancellationToken cancellationToken = default);
}

public class MeResponse
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public Guid UserId { get; set; }
}

public class MeHandler(MiniGramContext context, ICurrentRequest currentRequest) : IMeHandler
{
    public async Task<IResult> Handle(CancellationToken cancellationToken = default)
    {
        if (currentRequest.UserId == default) return Results.Forbid();

        var user = await context.Users.FindAsync([currentRequest.UserId], cancellationToken);
        if (user == null) return Results.NotFound($"No user with id: {currentRequest.UserId} could be found");

        return Results.Ok(new MeResponse
        {
            Email = user.Email,
            FullName = currentRequest.FullName,
            UserId = currentRequest.UserId,
            UserName = currentRequest.Username
        });
    }
}
