using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Data;

public class Comment
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    [Required, MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool Approved { get; set; } = true;
}
