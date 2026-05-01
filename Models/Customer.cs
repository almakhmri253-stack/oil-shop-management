using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم العميل مطلوب")]
    [StringLength(200)]
    [Display(Name = "اسم العميل")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "رقم الجوال")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    [Display(Name = "البريد الإلكتروني")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [StringLength(200)]
    [Display(Name = "نوع السيارة")]
    public string? CarModel { get; set; }

    [StringLength(20)]
    [Display(Name = "رقم اللوحة")]
    public string? PlateNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
