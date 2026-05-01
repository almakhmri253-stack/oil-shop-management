using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public enum PurchaseStatus
{
    Draft,
    Completed,
    Cancelled
}

public class Purchase
{
    public int Id { get; set; }

    [Display(Name = "رقم فاتورة الشراء")]
    public string PurchaseNumber { get; set; } = string.Empty;

    [Display(Name = "المورد")]
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    [Display(Name = "تاريخ الشراء")]
    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "الإجمالي قبل الضريبة")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "نسبة الضريبة (%)")]
    public decimal TaxRate { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "قيمة الضريبة")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "الخصم")]
    public decimal Discount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "الإجمالي النهائي")]
    public decimal TotalAmount { get; set; }

    [Display(Name = "طريقة الدفع")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [Display(Name = "الحالة")]
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Completed;

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    public ICollection<PurchaseReturn> Returns { get; set; } = new List<PurchaseReturn>();
}
