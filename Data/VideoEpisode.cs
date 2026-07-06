using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Data;

public class VideoEpisode
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }

    public int Order { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string VideoFileName { get; set; } = string.Empty;

    public int DurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string DurationFormatted =>
        DurationSeconds >= 3600
            ? TimeSpan.FromSeconds(DurationSeconds).ToString(@"h\:mm\:ss")
            : TimeSpan.FromSeconds(DurationSeconds).ToString(@"m\:ss");
}
