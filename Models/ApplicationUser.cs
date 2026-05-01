using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    [Display(Name = "ط§ظ„ط§ط³ظ… ط§ظ„ظƒط§ظ…ظ„")]
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    [StringLength(10)]
    public string? PreferredLanguage { get; set; }
}


