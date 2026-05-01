using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ط§ط³ظ… ط§ظ„طھطµظ†ظٹظپ ظ…ط·ظ„ظˆط¨")]
    [StringLength(100)]
    [Display(Name = "ط§ط³ظ… ط§ظ„طھطµظ†ظٹظپ")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "ط§ظ„ظˆطµظپ")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

