namespace OilShopManagement.Authorization;

public static class Permissions
{
    // المنتجات
    public const string ProductsView   = "Permissions.Products.View";
    public const string ProductsCreate = "Permissions.Products.Create";
    public const string ProductsEdit   = "Permissions.Products.Edit";
    public const string ProductsDelete = "Permissions.Products.Delete";

    // المخزون
    public const string StockView   = "Permissions.Stock.View";
    public const string StockAdjust = "Permissions.Stock.Adjust";

    // الفواتير
    public const string InvoicesView   = "Permissions.Invoices.View";
    public const string InvoicesCreate = "Permissions.Invoices.Create";
    public const string InvoicesCancel = "Permissions.Invoices.Cancel";
    public const string InvoicesPrint  = "Permissions.Invoices.Print";

    // العملاء
    public const string CustomersView   = "Permissions.Customers.View";
    public const string CustomersCreate = "Permissions.Customers.Create";
    public const string CustomersEdit   = "Permissions.Customers.Edit";
    public const string CustomersDelete = "Permissions.Customers.Delete";

    // التقارير
    public const string ReportsView       = "Permissions.Reports.View";
    public const string ReportsExport     = "Permissions.Reports.Export";

    // المستخدمون
    public const string UsersManage = "Permissions.Users.Manage";

    // مجموعات الصلاحيات حسب الوحدة
    public static readonly PermissionGroup[] Groups =
    [
        new("المنتجات", "bi-box-seam", "primary",
        [
            new(ProductsView,   "عرض المنتجات"),
            new(ProductsCreate, "إضافة منتج"),
            new(ProductsEdit,   "تعديل منتج"),
            new(ProductsDelete, "حذف منتج"),
        ]),
        new("المخزون", "bi-arrow-left-right", "info",
        [
            new(StockView,   "عرض حركة المخزون"),
            new(StockAdjust, "تعديل المخزون"),
        ]),
        new("الفواتير", "bi-receipt", "success",
        [
            new(InvoicesView,   "عرض الفواتير"),
            new(InvoicesCreate, "إنشاء فاتورة"),
            new(InvoicesCancel, "إلغاء فاتورة"),
            new(InvoicesPrint,  "طباعة الفاتورة"),
        ]),
        new("العملاء", "bi-people", "warning",
        [
            new(CustomersView,   "عرض العملاء"),
            new(CustomersCreate, "إضافة عميل"),
            new(CustomersEdit,   "تعديل عميل"),
            new(CustomersDelete, "حذف عميل"),
        ]),
        new("التقارير", "bi-bar-chart", "danger",
        [
            new(ReportsView,   "عرض التقارير"),
            new(ReportsExport, "تصدير التقارير"),
        ]),
        new("المستخدمون", "bi-shield-lock", "dark",
        [
            new(UsersManage, "إدارة المستخدمين والصلاحيات"),
        ]),
    ];

    // الصلاحيات الافتراضية للموظف
    public static readonly string[] EmployeeDefaults =
    [
        ProductsView,
        StockView,
        InvoicesView, InvoicesCreate, InvoicesPrint,
        CustomersView, CustomersCreate, CustomersEdit,
        ReportsView,
    ];

    // كل الصلاحيات (للمدير)
    public static IEnumerable<string> All =>
        Groups.SelectMany(g => g.Items).Select(i => i.Value);
}

public record PermissionGroup(string Label, string Icon, string Color, PermissionItem[] Items);
public record PermissionItem(string Value, string Label);
