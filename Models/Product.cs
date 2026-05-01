using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    [StringLength(200)]
    [Display(Name = "اسم المنتج")]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "رمز المنتج (SKU)")]
    public string? SKU { get; set; }

    [StringLength(500)]
    [Display(Name = "الوصف")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "التصنيف مطلوب")]
    [Display(Name = "التصنيف")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Required(ErrorMessage = "سعر الشراء مطلوب")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "سعر الشراء")]
    [Range(0, 999999)]
    public decimal PurchasePrice { get; set; }

    [Required(ErrorMessage = "سعر البيع مطلوب")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "سعر البيع")]
    [Range(0, 999999)]
    public decimal SalePrice { get; set; }

    [Display(Name = "الكمية الحالية")]
    public int CurrentStock { get; set; } = 0;

    [Display(Name = "الحد الأدنى للمخزون")]
    public int MinStockLevel { get; set; } = 5;

    [StringLength(50)]
    [Display(Name = "الوحدة")]
    public string Unit { get; set; } = "قطعة";

    [Display(Name = "عدد الحبات في الوحدة")]
    [Range(0, 99999)]
    public int? PiecesPerUnit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "عدد اللتر للحبة")]
    [Range(0, 9999)]
    public decimal? LitersPerPiece { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    [Display(Name = "سعر شراء الكرتون")]
    public decimal? CartonPurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    [Display(Name = "سعر بيع الكرتون")]
    public decimal? CartonSalePrice { get; set; }

    [StringLength(100)]
    [Display(Name = "الشركة / العلامة التجارية")]
    public string? Brand { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    [NotMapped]
    public bool IsLowStock => CurrentStock <= MinStockLevel;

    [NotMapped]
    public decimal ProfitMargin => SalePrice > 0 ? ((SalePrice - PurchasePrice) / SalePrice) * 100 : 0;
}
