namespace MiniGram.Api.Storage.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid PhotoId { get; set; }
    public int Votes { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public Guid UserId { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Photo Photo { get; set; }
}
