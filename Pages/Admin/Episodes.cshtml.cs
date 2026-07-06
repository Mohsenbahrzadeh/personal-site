using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Data;

namespace PersonalSite.Pages.Admin;

[RequestSizeLimit(1_073_741_824)] // 1 GB
[RequestFormLimits(MultipartBodyLengthLimit = 1_073_741_824)]
public class EpisodesModel(ApplicationDbContext db, IWebHostEnvironment env) : PageModel
{
    public Post Post { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "عنوان قسمت الزامی است"), MaxLength(200)]
        [Display(Name = "عنوان قسمت")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "توضیح قسمت الزامی است"), MaxLength(2000)]
        [Display(Name = "این قسمت در مورد چیست")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "فایل ویدیو (mp4)")]
        public IFormFile? VideoFile { get; set; }

        [Display(Name = "مدت زمان دستی (ثانیه) — فقط اگر تشخیص خودکار ممکن نبود")]
        public int? ManualDurationSeconds { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int postId)
    {
        var post = await db.Posts.Include(p => p.Episodes.OrderBy(e => e.Order)).FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null)
        {
            return NotFound();
        }
        Post = post;
        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int postId)
    {
        var post = await db.Posts.Include(p => p.Episodes).FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null)
        {
            return NotFound();
        }

        if (Input.VideoFile is null && Input.ManualDurationSeconds is null)
        {
            ModelState.AddModelError(nameof(Input.VideoFile), "فایل ویدیو را انتخاب کنید");
        }

        if (!ModelState.IsValid)
        {
            Post = post;
            return Page();
        }

        var episode = new VideoEpisode
        {
            PostId = post.Id,
            Title = Input.Title,
            Description = Input.Description,
            Order = post.Episodes.Count == 0 ? 1 : post.Episodes.Max(e => e.Order) + 1
        };

        if (Input.VideoFile is { Length: > 0 } file)
        {
            var folder = Path.Combine(env.WebRootPath, "videos", post.Slug);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}.mp4";
            var fullPath = Path.Combine(folder, fileName);

            await using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            episode.VideoFileName = $"{post.Slug}/{fileName}";

            await using var readStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var duration = Mp4DurationReader.TryReadDuration(readStream);
            episode.DurationSeconds = duration is not null
                ? (int)Math.Round(duration.Value.TotalSeconds)
                : Input.ManualDurationSeconds ?? 0;
        }
        else
        {
            episode.VideoFileName = string.Empty;
            episode.DurationSeconds = Input.ManualDurationSeconds ?? 0;
        }

        db.VideoEpisodes.Add(episode);
        await db.SaveChangesAsync();

        return RedirectToPage(new { postId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int postId, int episodeId)
    {
        var episode = await db.VideoEpisodes.FindAsync(episodeId);
        if (episode is not null)
        {
            if (!string.IsNullOrEmpty(episode.VideoFileName))
            {
                var fullPath = Path.Combine(env.WebRootPath, "videos", episode.VideoFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            db.VideoEpisodes.Remove(episode);
            await db.SaveChangesAsync();
        }

        return RedirectToPage(new { postId });
    }

    public async Task<IActionResult> OnPostMoveAsync(int postId, int episodeId, int direction)
    {
        var episodes = await db.VideoEpisodes.Where(e => e.PostId == postId).OrderBy(e => e.Order).ToListAsync();
        var index = episodes.FindIndex(e => e.Id == episodeId);
        var swapIndex = index + direction;

        if (index >= 0 && swapIndex >= 0 && swapIndex < episodes.Count)
        {
            (episodes[index].Order, episodes[swapIndex].Order) = (episodes[swapIndex].Order, episodes[index].Order);
            await db.SaveChangesAsync();
        }

        return RedirectToPage(new { postId });
    }
}
