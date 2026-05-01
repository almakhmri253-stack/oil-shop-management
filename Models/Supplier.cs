using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ط§ط³ظ… ط§ظ„ظ…ظˆط±ط¯ ظ…ط·ظ„ظˆط¨")]
    [StringLength(200)]
    [Display(Name = "ط§ط³ظ… ط§ظ„ظ…ظˆط±ط¯")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "ط§ظ„ظ‡ط§طھظپ")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [EmailAddress]
    [Display(Name = "ط§ظ„ط¨ط±ظٹط¯ ط§ظ„ط¥ظ„ظƒطھط±ظˆظ†ظٹ")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "ط§ظ„ط¹ظ†ظˆط§ظ†")]
    public string? Address { get; set; }

    [StringLength(200)]
    [Display(Name = "ط¬ظ‡ط© ط§ظ„ط§طھطµط§ظ„")]
    public string? ContactPerson { get; set; }

    [Display(Name = "ظ†ط´ط·")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();
}


