using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class PurchaseReturn
{
    public int Id { get; set; }

    [Display(Name = "رقم المرتجع")]
    public string ReturnNumber { get; set; } = string.Empty;

    [Display(Name = "فاتورة الشراء")]
    public int? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }

    [Display(Name = "المورد")]
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

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

    public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
}
