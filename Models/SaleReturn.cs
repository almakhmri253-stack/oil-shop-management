using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class SaleReturn
{
    public int Id { get; set; }

    [Display(Name = "رقم المرتجع")]
    public string ReturnNumber { get; set; } = string.Empty;

    [Display(Name = "فاتورة البيع")]
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    [Display(Name = "العميل")]
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Display(Name = "تاريخ الإرجاع")]
    public DateTime ReturnDate { get; set; } = DateTime.Now;

    [StringLength(500)]
    [Display(Name = "سبب الإرجاع")]
    public string? Reason { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "الإجمالي")]
    public decimal TotalAmount { get; set; }

    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<SaleReturnItem> Items { get; set; } = new List<SaleReturnItem>();
}
