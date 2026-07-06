using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Data;

namespace PersonalSite.Pages.Admin;

public class CommentsModel(ApplicationDbContext db) : PageModel
{
    public List<Comment> Comments { get; set; } = [];

    public async Task OnGetAsync()
    {
        Comments = await db.Comments
            .Include(c => c.Post)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleApproveAsync(int id)
    {
        var comment = await db.Comments.FindAsync(id);
        if (comment is not null)
        {
            comment.Approved = !comment.Approved;
            await db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var comment = await db.Comments.FindAsync(id);
        if (comment is not null)
        {
            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
