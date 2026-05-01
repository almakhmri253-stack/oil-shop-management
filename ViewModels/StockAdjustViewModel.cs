using Microsoft.AspNetCore.Mvc.Rendering;
using OilShopManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.ViewModels;

public class StockAdjustViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Display(Name = "ظ†ظˆط¹ ط§ظ„ط­ط±ظƒط©")]
    public TransactionType Type { get; set; }

    [Required]
    [Display(Name = "ط§ظ„ظƒظ…ظٹط©")]
    [Range(1, 9999, ErrorMessage = "ط§ظ„ظƒظ…ظٹط© ظٹط¬ط¨ ط£ظ† طھظƒظˆظ† ط¨ظٹظ† 1 ظˆ 9999")]
    public int Quantity { get; set; }

    [Display(Name = "ط³ط¹ط± ط§ظ„ظˆط­ط¯ط©")]
    [Range(0, 999999)]
    public decimal UnitPrice { get; set; }

    [StringLength(500)]
    [Display(Name = "ظ…ظ„ط§ط­ط¸ط§طھ")]
    public string? Notes { get; set; }

    public SelectList? Products { get; set; }
    public string? ProductName { get; set; }
    public int? CurrentStock { get; set; }
}

