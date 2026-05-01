using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.ViewModels;

public class ReturnItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public class PurchaseReturnCreateViewModel
{
    public int? PurchaseId { get; set; }
    public int? SupplierId { get; set; }

    [Display(Name = "تاريخ الإرجاع")]
    public DateTime ReturnDate { get; set; } = DateTime.Now;

    [StringLength(500)]
    [Display(Name = "سبب الإرجاع")]
    public string? Reason { get; set; }

    public decimal? ManualTotal { get; set; }
    public List<ReturnItemViewModel> Items { get; set; } = [];
}

public class SaleReturnCreateViewModel
{
    public int? InvoiceId { get; set; }
    public int? CustomerId { get; set; }

    [Display(Name = "تاريخ الإرجاع")]
    public DateTime ReturnDate { get; set; } = DateTime.Now;

    [StringLength(500)]
    [Display(Name = "سبب الإرجاع")]
    public string? Reason { get; set; }

    public decimal? ManualTotal { get; set; }
    public List<ReturnItemViewModel> Items { get; set; } = [];
}
