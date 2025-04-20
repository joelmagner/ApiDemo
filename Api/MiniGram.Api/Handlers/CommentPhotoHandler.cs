using Microsoft.EntityFrameworkCore;
using MiniGram.Api.Common;
using MiniGram.Api.Storage;
using MiniGram.Api.Storage.Entities;
using MiniGram.Api.Utils;

namespace MiniGram.Api.Handlers;

public interface ICommentPhotoHandler
{
    Task<IResult> Handle(CommentPhotoRequest request, CancellationToken cancellationToken = default);
}

public class CommentPhotoRequest
{
    public Guid PhotoId { get; set; }
    public string Comment { get; set; }
}

public class CommentPhotoHandler(MiniGramContext context, IValidator validator, ICurrentRequest currentRequest): ICommentPhotoHandler
{
    public async Task<IResult> Handle(CommentPhotoRequest request, CancellationToken cancellationToken = default)
    {

        var validated = ValidateRequest(request);
        if (!validated.IsValid)
        {
            return Results.ValidationProblem(validated.Errors);
        }
        
        var photo = await context.Photos.FirstOrDefaultAsync(photo => photo.Id == request.PhotoId, cancellationToken);
        if (photo == null) return Results.NotFound($"No photo with id: {request.PhotoId} could be found");
        
        var comment = new Comment
        {
            PhotoId = request.PhotoId,
            Text = request.Comment,
            Created = DateTime.UtcNow,
            UserId = currentRequest.UserId
        };
     
        // context.Comments.Add(comment);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Created($"/photos/{request.PhotoId}/comments/{comment.PhotoId}", comment);
    }
    
    IValidator ValidateRequest(CommentPhotoRequest request)
    {
        validator.Validate(request);
        validator.Check(request.Comment, nameof(request.Comment), $"{nameof(request.Comment)} is required.");
        validator.Check(request.Comment, nameof(request.Comment), $"{nameof(request.Comment)} is too long.", s => s.Length <= 500);
        return validator;
    }
}