using Microsoft.AspNetCore.Mvc.Rendering;
using OilShopManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.ViewModels;

public class PurchaseCreateViewModel
{
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }

    [Display(Name = "طھط§ط±ظٹط® ط§ظ„ط´ط±ط§ط،")]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "ط·ط±ظٹظ‚ط© ط§ظ„ط¯ظپط¹")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [Display(Name = "ظ†ط³ط¨ط© ط§ظ„ط¶ط±ظٹط¨ط© (%)")]
    [Range(0, 100)]
    public decimal TaxRate { get; set; } = 0;

    [Display(Name = "ط§ظ„ط®طµظ…")]
    [Range(0, 999999)]
    public decimal Discount { get; set; } = 0;

    [StringLength(500)]
    [Display(Name = "ظ…ظ„ط§ط­ط¸ط§طھ")]
    public string? Notes { get; set; }

    public decimal? ManualSubTotal { get; set; }

    public List<PurchaseItemViewModel> Items { get; set; } = [];

    public SelectList? Suppliers { get; set; }
}

public class PurchaseItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }        // always in pieces for carton products
    public decimal UnitPrice { get; set; }   // always piece price
    public decimal Total { get; set; }
    public int PiecesPerUnit { get; set; }   // > 0 means carton product
}


