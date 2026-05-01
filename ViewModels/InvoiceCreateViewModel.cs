using Microsoft.AspNetCore.Mvc.Rendering;
using OilShopManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.ViewModels;

public class InvoiceCreateViewModel
{
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }

    [Display(Name = "تاريخ الفاتورة")]
    public DateTime InvoiceDate { get; set; } = DateTime.Now;

    [Display(Name = "طريقة الدفع")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [Display(Name = "نسبة الضريبة (%)")]
    [Range(0, 100)]
    public decimal TaxRate { get; set; } = 15;

    [Display(Name = "الخصم")]
    [Range(0, 999999)]
    public decimal Discount { get; set; } = 0;

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    public decimal? ManualSubTotal { get; set; }

    public List<InvoiceItemViewModel> Items { get; set; } = [];

    public SelectList? Customers { get; set; }
    public SelectList? Products { get; set; }
}

public class InvoiceItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal Total { get; set; }
}
