using Microsoft.EntityFrameworkCore;
using MiniGram.Api.Storage;
using MiniGram.Api.Utils;

namespace MiniGram.Api.Handlers;

public interface IGetUserPhotosHandler
{
    Task<IResult> Handle(Guid userId, CancellationToken cancellationToken = default);
}

public class GetUserPhotosHandlerHandler(MiniGramContext context, IValidator validator) : IGetUserPhotosHandler
{
    public async Task<IResult> Handle(Guid userId, CancellationToken cancellationToken = default)
    {
        var validated = ValidateRequest(userId);
        if (!validated.IsValid) return Results.ValidationProblem(validated.Errors);

        var photos = await context.Photos
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
        return Results.Ok(photos);
    }

    IValidator ValidateRequest(Guid userId)
    {
        validator.Validate(userId);
        validator.Check(userId.ToString(), nameof(userId), $"{nameof(userId)} is required.");

        return validator;
    }
}
