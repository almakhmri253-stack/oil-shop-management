using System.ComponentModel.DataAnnotations;

namespace OilShopManagement.Models;

public class Supplier
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم المورد مطلوب")]
    [StringLength(200)]
    [Display(Name = "اسم المورد")]
    public string Name { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "الهاتف")]
    public string? Phone { get; set; }

    [StringLength(100)]
    [EmailAddress]
    [Display(Name = "البريد الإلكتروني")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    [StringLength(200)]
    [Display(Name = "جهة الاتصال")]
    public string? ContactPerson { get; set; }

    [Display(Name = "نشط")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();
}
