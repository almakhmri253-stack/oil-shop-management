namespace OilShopManagement.Authorization;

public static class Permissions
{
    // ط§ظ„ظ…ظ†طھط¬ط§طھ
    public const string ProductsView   = "Permissions.Products.View";
    public const string ProductsCreate = "Permissions.Products.Create";
    public const string ProductsEdit   = "Permissions.Products.Edit";
    public const string ProductsDelete = "Permissions.Products.Delete";

    // ط§ظ„ظ…ط®ط²ظˆظ†
    public const string StockView   = "Permissions.Stock.View";
    public const string StockAdjust = "Permissions.Stock.Adjust";

    // ط§ظ„ظپظˆط§طھظٹط±
    public const string InvoicesView   = "Permissions.Invoices.View";
    public const string InvoicesCreate = "Permissions.Invoices.Create";
    public const string InvoicesCancel = "Permissions.Invoices.Cancel";
    public const string InvoicesPrint  = "Permissions.Invoices.Print";

    // ط§ظ„ط¹ظ…ظ„ط§ط،
    public const string CustomersView   = "Permissions.Customers.View";
    public const string CustomersCreate = "Permissions.Customers.Create";
    public const string CustomersEdit   = "Permissions.Customers.Edit";
    public const string CustomersDelete = "Permissions.Customers.Delete";

    // ط§ظ„طھظ‚ط§ط±ظٹط±
    public const string ReportsView       = "Permissions.Reports.View";
    public const string ReportsExport     = "Permissions.Reports.Export";

    // ط§ظ„ظ…ط³طھط®ط¯ظ…ظˆظ†
    public const string UsersManage = "Permissions.Users.Manage";

    // ظ…ط¬ظ…ظˆط¹ط§طھ ط§ظ„طµظ„ط§ط­ظٹط§طھ ط­ط³ط¨ ط§ظ„ظˆط­ط¯ط©
    public static readonly PermissionGroup[] Groups =
    [
        new("ط§ظ„ظ…ظ†طھط¬ط§طھ", "bi-box-seam", "primary",
        [
            new(ProductsView,   "ط¹ط±ط¶ ط§ظ„ظ…ظ†طھط¬ط§طھ"),
            new(ProductsCreate, "ط¥ط¶ط§ظپط© ظ…ظ†طھط¬"),
            new(ProductsEdit,   "طھط¹ط¯ظٹظ„ ظ…ظ†طھط¬"),
            new(ProductsDelete, "ط­ط°ظپ ظ…ظ†طھط¬"),
        ]),
        new("ط§ظ„ظ…ط®ط²ظˆظ†", "bi-arrow-left-right", "info",
        [
            new(StockView,   "ط¹ط±ط¶ ط­ط±ظƒط© ط§ظ„ظ…ط®ط²ظˆظ†"),
            new(StockAdjust, "طھط¹ط¯ظٹظ„ ط§ظ„ظ…ط®ط²ظˆظ†"),
        ]),
        new("ط§ظ„ظپظˆط§طھظٹط±", "bi-receipt", "success",
        [
            new(InvoicesView,   "ط¹ط±ط¶ ط§ظ„ظپظˆط§طھظٹط±"),
            new(InvoicesCreate, "ط¥ظ†ط´ط§ط، ظپط§طھظˆط±ط©"),
            new(InvoicesCancel, "ط¥ظ„ط؛ط§ط، ظپط§طھظˆط±ط©"),
            new(InvoicesPrint,  "ط·ط¨ط§ط¹ط© ط§ظ„ظپط§طھظˆط±ط©"),
        ]),
        new("ط§ظ„ط¹ظ…ظ„ط§ط،", "bi-people", "warning",
        [
            new(CustomersView,   "ط¹ط±ط¶ ط§ظ„ط¹ظ…ظ„ط§ط،"),
            new(CustomersCreate, "ط¥ط¶ط§ظپط© ط¹ظ…ظٹظ„"),
            new(CustomersEdit,   "طھط¹ط¯ظٹظ„ ط¹ظ…ظٹظ„"),
            new(CustomersDelete, "ط­ط°ظپ ط¹ظ…ظٹظ„"),
        ]),
        new("ط§ظ„طھظ‚ط§ط±ظٹط±", "bi-bar-chart", "danger",
        [
            new(ReportsView,   "ط¹ط±ط¶ ط§ظ„طھظ‚ط§ط±ظٹط±"),
            new(ReportsExport, "طھطµط¯ظٹط± ط§ظ„طھظ‚ط§ط±ظٹط±"),
        ]),
        new("ط§ظ„ظ…ط³طھط®ط¯ظ…ظˆظ†", "bi-shield-lock", "dark",
        [
            new(UsersManage, "ط¥ط¯ط§ط±ط© ط§ظ„ظ…ط³طھط®ط¯ظ…ظٹظ† ظˆط§ظ„طµظ„ط§ط­ظٹط§طھ"),
        ]),
    ];

    // ط§ظ„طµظ„ط§ط­ظٹط§طھ ط§ظ„ط§ظپطھط±ط§ط¶ظٹط© ظ„ظ„ظ…ظˆط¸ظپ
    public static readonly string[] EmployeeDefaults =
    [
        ProductsView,
        StockView,
        InvoicesView, InvoicesCreate, InvoicesPrint,
        CustomersView, CustomersCreate, CustomersEdit,
        ReportsView,
    ];

    // ظƒظ„ ط§ظ„طµظ„ط§ط­ظٹط§طھ (ظ„ظ„ظ…ط¯ظٹط±)
    public static IEnumerable<string> All =>
        Groups.SelectMany(g => g.Items).Select(i => i.Value);
}

public record PermissionGroup(string Label, string Icon, string Color, PermissionItem[] Items);
public record PermissionItem(string Value, string Label);


