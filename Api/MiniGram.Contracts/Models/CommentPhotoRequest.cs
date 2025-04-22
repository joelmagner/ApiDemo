namespace MiniGram.Contracts.Models;

public class CommentPhotoRequest
{
    public Guid PhotoId { get; set; }
    public string Comment { get; set; }
}
