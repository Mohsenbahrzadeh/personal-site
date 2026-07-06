using Microsoft.AspNetCore.Identity;

namespace PersonalSite.Data;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}
