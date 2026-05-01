using Microsoft.AspNetCore.Mvc.Rendering;
using OilShopManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.ViewModels;

public class StockAdjustViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Display(Name = "نوع الحركة")]
    public TransactionType Type { get; set; }

    [Required]
    [Display(Name = "الكمية")]
    [Range(1, 9999, ErrorMessage = "الكمية يجب أن تكون بين 1 و 9999")]
    public int Quantity { get; set; }

    [Display(Name = "سعر الوحدة")]
    [Range(0, 999999)]
    public decimal UnitPrice { get; set; }

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    public SelectList? Products { get; set; }
    public string? ProductName { get; set; }
    public int? CurrentStock { get; set; }
}
