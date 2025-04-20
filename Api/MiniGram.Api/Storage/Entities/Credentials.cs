namespace MiniGram.Api.Storage.Entities;

public class Credentials
{
    public Guid UserId { get; set; }
    public string Password { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}