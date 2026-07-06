using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Data;

namespace PersonalSite.Pages.Admin;

public class EditModel(ApplicationDbContext db) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int? Id { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "عنوان الزامی است"), MaxLength(200)]
        [Display(Name = "عنوان")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "نامک الزامی است"), MaxLength(200)]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "نامک فقط می‌تواند شامل حروف انگلیسی کوچک، عدد و خط تیره باشد")]
        [Display(Name = "نامک (Slug)")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "خلاصه الزامی است"), MaxLength(400)]
        [Display(Name = "خلاصه")]
        public string Summary { get; set; } = string.Empty;

        [Required(ErrorMessage = "متن الزامی است")]
        [Display(Name = "متن (HTML مجاز است)")]
        public string Body { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "برچسب‌ها (با کاما جدا کنید)")]
        public string Tags { get; set; } = string.Empty;

        [MaxLength(4)]
        [Display(Name = "آیکون")]
        public string Icon { get; set; } = "📘";

        [Range(0, 5)]
        [Display(Name = "رنگ کارت (۰ تا ۵)")]
        public int AccentIndex { get; set; }

        [Display(Name = "منتشرشده")]
        public bool Published { get; set; } = true;

        [Display(Name = "نوع مطلب")]
        public PostKind Kind { get; set; } = PostKind.Article;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        Id = id;
        if (id is not null)
        {
            var post = await db.Posts.FindAsync(id.Value);
            if (post is null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Title = post.Title,
                Slug = post.Slug,
                Summary = post.Summary,
                Body = post.Body,
                Tags = post.Tags,
                Icon = post.Icon,
                AccentIndex = post.AccentIndex,
                Published = post.Published,
                Kind = post.Kind
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        Id = id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var slugTaken = await db.Posts.AnyAsync(p => p.Slug == Input.Slug && p.Id != (id ?? 0));
        if (slugTaken)
        {
            ModelState.AddModelError(nameof(Input.Slug), "این نامک قبلاً استفاده شده است");
            return Page();
        }

        Post post;
        if (id is not null)
        {
            var existing = await db.Posts.FindAsync(id.Value);
            if (existing is null)
            {
                return NotFound();
            }
            post = existing;
        }
        else
        {
            post = new Post { CreatedAt = DateTime.UtcNow };
            db.Posts.Add(post);
        }

        post.Title = Input.Title;
        post.Slug = Input.Slug;
        post.Summary = Input.Summary;
        post.Body = Input.Body;
        post.Tags = Input.Tags;
        post.Icon = Input.Icon;
        post.AccentIndex = Input.AccentIndex;
        post.Published = Input.Published;
        post.Kind = Input.Kind;

        await db.SaveChangesAsync();

        if (post.Kind == PostKind.VideoCourse)
        {
            return RedirectToPage("./Episodes", new { postId = post.Id });
        }

        return RedirectToPage("./Index");
    }
}
