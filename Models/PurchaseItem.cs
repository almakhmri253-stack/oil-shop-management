using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class PurchaseItem
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [StringLength(200)]
    [Display(Name = "ط§ظ„ظ…ظ†طھط¬")]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, 9999)]
    [Display(Name = "ط§ظ„ظƒظ…ظٹط©")]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط³ط¹ط± ط§ظ„ظˆط­ط¯ط©")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ")]
    public decimal Total { get; set; }
}


