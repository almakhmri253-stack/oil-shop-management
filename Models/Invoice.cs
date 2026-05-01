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

    [Display(Name = "ط±ظ‚ظ… ط§ظ„ظپط§طھظˆط±ط©")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Display(Name = "ط§ظ„ط¹ظ…ظٹظ„")]
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Display(Name = "طھط§ط±ظٹط® ط§ظ„ظپط§طھظˆط±ط©")]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ ظ‚ط¨ظ„ ط§ظ„ط¶ط±ظٹط¨ط©")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ظ†ط³ط¨ط© ط§ظ„ط¶ط±ظٹط¨ط© (%)")]
    public decimal TaxRate { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ظ‚ظٹظ…ط© ط§ظ„ط¶ط±ظٹط¨ط©")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط®طµظ…")]
    public decimal Discount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط§ظ„ط¥ط¬ظ…ط§ظ„ظٹ ط§ظ„ظ†ظ‡ط§ط¦ظٹ")]
    public decimal TotalAmount { get; set; }

    [Display(Name = "ط·ط±ظٹظ‚ط© ط§ظ„ط¯ظپط¹")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [Display(Name = "ط­ط§ظ„ط© ط§ظ„ظپط§طھظˆط±ط©")]
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Completed;

    [StringLength(500)]
    [Display(Name = "ظ…ظ„ط§ط­ط¸ط§طھ")]
    public string? Notes { get; set; }

    [Display(Name = "ط§ظ„ظ…ط³طھط®ط¯ظ…")]
    public string? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}

