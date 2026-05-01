using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ط§ط³ظ… ط§ظ„ط¹ظ…ظٹظ„ ظ…ط·ظ„ظˆط¨")]
    [StringLength(200)]
    [Display(Name = "ط§ط³ظ… ط§ظ„ط¹ظ…ظٹظ„")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "ط±ظ‚ظ… ط§ظ„ط¬ظˆط§ظ„")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [EmailAddress(ErrorMessage = "ط§ظ„ط¨ط±ظٹط¯ ط§ظ„ط¥ظ„ظƒطھط±ظˆظ†ظٹ ط؛ظٹط± طµط­ظٹط­")]
    [Display(Name = "ط§ظ„ط¨ط±ظٹط¯ ط§ظ„ط¥ظ„ظƒطھط±ظˆظ†ظٹ")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "ط§ظ„ط¹ظ†ظˆط§ظ†")]
    public string? Address { get; set; }

    [StringLength(200)]
    [Display(Name = "ظ†ظˆط¹ ط§ظ„ط³ظٹط§ط±ط©")]
    public string? CarModel { get; set; }

    [StringLength(20)]
    [Display(Name = "ط±ظ‚ظ… ط§ظ„ظ„ظˆط­ط©")]
    public string? PlateNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

