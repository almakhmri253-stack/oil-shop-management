using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class SaleReturn
{
    public int Id { get; set; }

    [Display(Name = "ط±ظ‚ظ… ط§ظ„ظ…ط±طھط¬ط¹")]
    public string ReturnNumber { get; set; } = string.Empty;

    [Display(Name = "ظپط§طھظˆط±ط© ط§ظ„ط¨ظٹط¹")]
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    [Display(Name = "ط§ظ„ط¹ظ…ظٹظ„")]
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

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

    public ICollection<SaleReturnItem> Items { get; set; } = new List<SaleReturnItem>();
}

