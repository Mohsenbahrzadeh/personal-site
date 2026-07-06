using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Data;

public class Post
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required, MaxLength(400)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Tags { get; set; } = string.Empty;

    [MaxLength(4)]
    public string Icon { get; set; } = "📘";

    public int AccentIndex { get; set; }

    public bool Published { get; set; } = true;

    public PostKind Kind { get; set; } = PostKind.Article;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Comment> Comments { get; set; } = [];

    public List<Rating> Ratings { get; set; } = [];

    public List<VideoEpisode> Episodes { get; set; } = [];

    public IEnumerable<string> TagList =>
        Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public int TotalDurationSeconds => Episodes.Sum(e => e.DurationSeconds);

    public string TotalDurationFormatted =>
        TotalDurationSeconds >= 3600
            ? TimeSpan.FromSeconds(TotalDurationSeconds).ToString(@"h\:mm\:ss")
            : TimeSpan.FromSeconds(TotalDurationSeconds).ToString(@"m\:ss");
}
