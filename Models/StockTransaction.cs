using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public enum TransactionType
{
    Purchase,
    Sale,
    Adjustment,
    Return
}

public class StockTransaction
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Display(Name = "ظ†ظˆط¹ ط§ظ„ط­ط±ظƒط©")]
    public TransactionType Type { get; set; }

    [Display(Name = "ط§ظ„ظƒظ…ظٹط©")]
    public int Quantity { get; set; }

    [Display(Name = "ط§ظ„ظƒظ…ظٹط© ظ‚ط¨ظ„")]
    public int QuantityBefore { get; set; }

    [Display(Name = "ط§ظ„ظƒظ…ظٹط© ط¨ط¹ط¯")]
    public int QuantityAfter { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط³ط¹ط± ط§ظ„ظˆط­ط¯ط©")]
    public decimal UnitPrice { get; set; }

    [StringLength(500)]
    [Display(Name = "ظ…ظ„ط§ط­ط¸ط§طھ")]
    public string? Notes { get; set; }

    public int? InvoiceId { get; set; }

    [Display(Name = "ط§ظ„ظ…ط³طھط®ط¯ظ…")]
    public string? UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

