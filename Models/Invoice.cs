using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public enum InvoiceStatus
{
    Draft,
    Completed,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Card,
    Transfer
}

public class Invoice
{
    public int Id { get; set; }

    [Display(Name = "رقم الفاتورة")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Display(Name = "العميل")]
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Display(Name = "تاريخ الفاتورة")]
    public DateTime InvoiceDate { get; set; } = DateTime.Now;

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

    [Display(Name = "حالة الفاتورة")]
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Completed;

    [StringLength(500)]
    [Display(Name = "ملاحظات")]
    public string? Notes { get; set; }

    [Display(Name = "المستخدم")]
    public string? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
