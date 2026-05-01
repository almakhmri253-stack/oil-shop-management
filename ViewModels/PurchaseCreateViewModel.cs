using Microsoft.AspNetCore.Mvc.Rendering;
using OilShopManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.ViewModels;

public class PurchaseCreateViewModel
{
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }

    [Display(Name = "تاريخ الشراء")]
    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    [Display(Name = "طريقة الدفع")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [Display(Name = "نسبة الضريبة (%)")]
    [Range(0, 100)]
    public decimal TaxRate { get; set; } = 0;

    [Display(Name = "الخصم")]
    [Range(0, 999999)]
    public decimal Discount { get; set; } = 0;

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
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
