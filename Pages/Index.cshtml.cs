using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Data;

namespace PersonalSite.Pages;

public class IndexModel(ApplicationDbContext db) : PageModel
{
    public List<Post> Posts { get; set; } = [];

    public async Task OnGetAsync()
    {
        Posts = await db.Posts
            .Where(p => p.Published)
            .Include(p => p.Ratings)
            .Include(p => p.Episodes)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}
