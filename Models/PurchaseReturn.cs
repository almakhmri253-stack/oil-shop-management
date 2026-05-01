using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class PurchaseReturn
{
    public int Id { get; set; }

    [Display(Name = "ط±ظ‚ظ… ط§ظ„ظ…ط±طھط¬ط¹")]
    public string ReturnNumber { get; set; } = string.Empty;

    [Display(Name = "ظپط§طھظˆط±ط© ط§ظ„ط´ط±ط§ط،")]
    public int? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }

    [Display(Name = "ط§ظ„ظ…ظˆط±ط¯")]
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    [Display(Name = "طھط§ط±ظٹط® ط§ظ„ط¥ط±ط¬ط§ط¹")]
    public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    [Display(Name = "ط³ط¨ط¨ ط§ظ„ط¥ط±ط¬ط§ط¹")]
    public string? Reason { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ")]
    public decimal TotalAmount { get; set; }

    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
}


