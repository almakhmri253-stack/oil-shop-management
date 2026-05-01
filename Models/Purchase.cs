using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public enum PurchaseStatus
{
    Draft,
    Completed,
    Cancelled
}

public class Purchase
{
    public int Id { get; set; }

    [Display(Name = "ط±ظ‚ظ… ظپط§طھظˆط±ط© ط§ظ„ط´ط±ط§ط،")]
    public string PurchaseNumber { get; set; } = string.Empty;

    [Display(Name = "ط§ظ„ظ…ظˆط±ط¯")]
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    [Display(Name = "طھط§ط±ظٹط® ط§ظ„ط´ط±ط§ط،")]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ ظ‚ط¨ظ„ ط§ظ„ط¶ط±ظٹط¨ط©")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ظ†ط³ط¨ط© ط§ظ„ط¶ط±ظٹط¨ط© (%)")]
    public decimal TaxRate { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ظ‚ظٹظ…ط© ط§ظ„ط¶ط±ظٹط¨ط©")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط®طµظ…")]
    public decimal Discount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ ط§ظ„ظ†ظ‡ط§ط¦ظٹ")]
    public decimal TotalAmount { get; set; }

    [Display(Name = "ط·ط±ظٹظ‚ط© ط§ظ„ط¯ظپط¹")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [Display(Name = "ط§ظ„ط­ط§ظ„ط©")]
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Completed;

    [StringLength(500)]
    [Display(Name = "ظ…ظ„ط§ط­ط¸ط§طھ")]
    public string? Notes { get; set; }

    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    public ICollection<PurchaseReturn> Returns { get; set; } = new List<PurchaseReturn>();
}

