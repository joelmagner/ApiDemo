namespace MiniGram.Api.Storage.Entities;

public class Photo
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Votes { get; set; }
    public byte[] Contents { get; set; }
    public string ContentType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
    
    // Navigation property
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public User User { get; set; }
}