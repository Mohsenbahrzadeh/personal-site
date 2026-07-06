using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Data;

namespace PersonalSite.Pages.Admin;

public class IndexModel(ApplicationDbContext db, IWebHostEnvironment env) : PageModel
{
    public List<Post> Posts { get; set; } = [];

    public async Task OnGetAsync()
    {
        Posts = await db.Posts
            .Include(p => p.Comments)
            .Include(p => p.Ratings)
            .Include(p => p.Episodes)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostTogglePublishAsync(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is not null)
        {
            post.Published = !post.Published;
            await db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var post = await db.Posts.FindAsync(id);
        if (post is not null)
        {
            db.Posts.Remove(post);
            await db.SaveChangesAsync();

            var videoFolder = Path.Combine(env.WebRootPath, "videos", post.Slug);
            if (Directory.Exists(videoFolder))
            {
                Directory.Delete(videoFolder, recursive: true);
            }
        }
        return RedirectToPage();
    }
}
