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

    [Display(Name = "نوع الحركة")]
    public TransactionType Type { get; set; }

    [Display(Name = "الكمية")]
    public int Quantity { get; set; }

    [Display(Name = "الكمية قبل")]
    public int QuantityBefore { get; set; }

    [Display(Name = "الكمية بعد")]
    public int QuantityAfter { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "سعر الوحدة")]
    public decimal UnitPrice { get; set; }

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    public int? InvoiceId { get; set; }

    [Display(Name = "المستخدم")]
    public string? UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
