using MiniGram.Api.Common;
using MiniGram.Api.Storage;
using MiniGram.Api.Storage.Entities;
using MiniGram.Api.Utils;

namespace MiniGram.Api.Handlers;


public interface IUploadPhotoHandler
{
    Task<IResult> Handle(UploadPhotoRequest request, CancellationToken cancellationToken = default);
}

public class UploadPhotoRequest
{
    public string ContentType { get; set; }
    public string? Description { get; set; }
    public byte[] Contents { get; set; }
}

public class UploadPhotoHandler(MiniGramContext context, ICurrentRequest currentRequest, IValidator validator): IUploadPhotoHandler
{
    public async Task<IResult> Handle(UploadPhotoRequest request, CancellationToken cancellationToken = default)
    {
        
        var validation = ValidateRequest(request);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.Errors);
        }
        
        
        var photo = new Photo
        {
            ContentType = request.ContentType ?? "image/jpeg",
            Contents = request.Contents,
            CreatedAt = DateTime.UtcNow,
            UserId = currentRequest.UserId,
            Description = request.Description
        };
        
        context.Photos.Add(photo);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Created($"/photos/{photo.Id}", photo);
    }
    
    IValidator ValidateRequest(UploadPhotoRequest request)
    {
        validator.Validate(request);
        validator.Check(request.Description, nameof(request.Description), $"{nameof(request.Description)} is required or too long.", s => s.Length == 0 || s.Length < 500);
        validator.Check(request.Contents, nameof(request.Contents), $"{nameof(request.Contents)} is too large.", s => s?.Length <= 1024*1024*5); // 5mb
        return validator;
    }
}