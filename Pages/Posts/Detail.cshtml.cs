using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Data;

namespace PersonalSite.Pages.Posts;

public class DetailModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : PageModel
{
    public Post Post { get; set; } = null!;
    public double AverageRating { get; set; }
    public int MyRating { get; set; }

    [BindProperty]
    public string CommentText { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var post = await db.Posts
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .Include(p => p.Ratings)
            .Include(p => p.Episodes)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Published);

        if (post is null)
        {
            return NotFound();
        }

        Post = post;
        AverageRating = post.Ratings.Count > 0 ? post.Ratings.Average(r => r.Score) : 0;

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = userManager.GetUserId(User);
            MyRating = post.Ratings.FirstOrDefault(r => r.UserId == userId)?.Score ?? 0;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRateAsync(string slug, int score)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (score is < 1 or > 5)
        {
            return BadRequest();
        }

        var post = await db.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        if (post is null)
        {
            return NotFound();
        }

        var userId = userManager.GetUserId(User)!;
        var existing = await db.Ratings.FirstOrDefaultAsync(r => r.PostId == post.Id && r.UserId == userId);
        if (existing is null)
        {
            db.Ratings.Add(new Rating { PostId = post.Id, UserId = userId, Score = score });
        }
        else
        {
            existing.Score = score;
        }

        await db.SaveChangesAsync();
        return RedirectToPage(new { slug });
    }

    public async Task<IActionResult> OnPostCommentAsync(string slug)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        var post = await db.Posts.FirstOrDefaultAsync(p => p.Slug == slug);
        if (post is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(CommentText))
        {
            var userId = userManager.GetUserId(User)!;
            db.Comments.Add(new Comment { PostId = post.Id, UserId = userId, Text = CommentText.Trim() });
            await db.SaveChangesAsync();
        }

        return RedirectToPage(new { slug });
    }
}
