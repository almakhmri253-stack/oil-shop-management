using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class InvoiceItem
{
    public int Id { get; set; }

    [Required]
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    [Required]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Display(Name = "ط§ط³ظ… ط§ظ„ظ…ظ†طھط¬")]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "ط§ظ„ظƒظ…ظٹط©")]
    [Range(1, 9999)]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط³ط¹ط± ط§ظ„ظˆط­ط¯ط©")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط³ط¹ط± ط§ظ„ط´ط±ط§ط،")]
    public decimal PurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ")]
    public decimal Total { get; set; }
}

