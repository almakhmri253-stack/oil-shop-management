using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OilShopManagement.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ط§ط³ظ… ط§ظ„ظ…ظ†طھط¬ ظ…ط·ظ„ظˆط¨")]
    [StringLength(200)]
    [Display(Name = "ط§ط³ظ… ط§ظ„ظ…ظ†طھط¬")]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "ط±ظ…ط² ط§ظ„ظ…ظ†طھط¬ (SKU)")]
    public string? SKU { get; set; }

    [StringLength(500)]
    [Display(Name = "ط§ظ„ظˆطµظپ")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "ط§ظ„طھطµظ†ظٹظپ ظ…ط·ظ„ظˆط¨")]
    [Display(Name = "ط§ظ„طھطµظ†ظٹظپ")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Required(ErrorMessage = "ط³ط¹ط± ط§ظ„ط´ط±ط§ط، ظ…ط·ظ„ظˆط¨")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط³ط¹ط± ط§ظ„ط´ط±ط§ط،")]
    [Range(0, 999999)]
    public decimal PurchasePrice { get; set; }

    [Required(ErrorMessage = "ط³ط¹ط± ط§ظ„ط¨ظٹط¹ ظ…ط·ظ„ظˆط¨")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط³ط¹ط± ط§ظ„ط¨ظٹط¹")]
    [Range(0, 999999)]
    public decimal SalePrice { get; set; }

    [Display(Name = "ط§ظ„ظƒظ…ظٹط© ط§ظ„ط­ط§ظ„ظٹط©")]
    public int CurrentStock { get; set; } = 0;

    [Display(Name = "ط§ظ„ط­ط¯ ط§ظ„ط£ط¯ظ†ظ‰ ظ„ظ„ظ…ط®ط²ظˆظ†")]
    public int MinStockLevel { get; set; } = 5;

    [StringLength(50)]
    [Display(Name = "ط§ظ„ظˆط­ط¯ط©")]
    public string Unit { get; set; } = "ظ‚ط·ط¹ط©";

    [Display(Name = "ط¹ط¯ط¯ ط§ظ„ط­ط¨ط§طھ ظپظٹ ط§ظ„ظˆط­ط¯ط©")]
    [Range(0, 99999)]
    public int? PiecesPerUnit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "ط¹ط¯ط¯ ط§ظ„ظ„طھط± ظ„ظ„ط­ط¨ط©")]
    [Range(0, 9999)]
    public decimal? LitersPerPiece { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    [Display(Name = "ط³ط¹ط± ط´ط±ط§ط، ط§ظ„ظƒط±طھظˆظ†")]
    public decimal? CartonPurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    [Display(Name = "ط³ط¹ط± ط¨ظٹط¹ ط§ظ„ظƒط±طھظˆظ†")]
    public decimal? CartonSalePrice { get; set; }

    [StringLength(100)]
    [Display(Name = "ط§ظ„ط´ط±ظƒط© / ط§ظ„ط¹ظ„ط§ظ…ط© ط§ظ„طھط¬ط§ط±ظٹط©")]
    public string? Brand { get; set; }

    [StringLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    [NotMapped]
    public bool IsLowStock => CurrentStock <= MinStockLevel;

    [NotMapped]
    public decimal ProfitMargin => SalePrice > 0 ? ((SalePrice - PurchasePrice) / SalePrice) * 100 : 0;
}

