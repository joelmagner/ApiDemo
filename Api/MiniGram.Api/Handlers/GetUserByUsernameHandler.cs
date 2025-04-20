using Microsoft.EntityFrameworkCore;
using MiniGram.Api.Common;
using MiniGram.Api.Storage;

namespace MiniGram.Api.Handlers;



public record GetUserByUsernameRequest(string Username);

public interface IGetUserByUsernameHandler
{
    Task<IResult> Handle(GetUserByUsernameRequest request, CancellationToken cancellationToken = default);
}



public class GetUserByUsernameHandler(MiniGramContext context, ICurrentRequest currentRequest): IGetUserByUsernameHandler
{
    public async Task<IResult> Handle(GetUserByUsernameRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentRequest.Token))
        {
            
        }
        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);
        if (user == null)
        {
            return Results.NotFound($"No user with {request.Username} could be found");
        }
        
        return Results.Ok(user);
    }

}