using System.Globalization;

namespace OilShopManagement.Services;

public class LangHelper
{
    public bool IsAr => CultureInfo.CurrentCulture.Name.StartsWith("ar");
    public string Dir  => IsAr ? "rtl" : "ltr";
    public string Lang => IsAr ? "ar" : "en";
    public string BootstrapCss => IsAr
        ? "https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.rtl.min.css"
        : "https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css";

    public string T(string arabic, string english) => IsAr ? arabic : english;
}


