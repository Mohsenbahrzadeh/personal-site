namespace PersonalSite.Data;

public class Rating
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int Score { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
