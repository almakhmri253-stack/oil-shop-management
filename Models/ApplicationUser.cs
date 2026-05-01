using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    [Display(Name = "الاسم الكامل")]
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? LastLoginAt { get; set; }

    [StringLength(10)]
    public string? PreferredLanguage { get; set; }
}
