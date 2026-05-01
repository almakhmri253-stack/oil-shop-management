using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OilShopManagement.Authorization;
using OilShopManagement.Models;
using System.Security.Claims;

namespace OilShopManagement.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string? search, string? roleFilter, string? statusFilter)
    {
        var users = await _userManager.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

        var result = new List<UserWithRolesViewModel>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            var permClaims = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
            result.Add(new UserWithRolesViewModel { User = user, Roles = roles, Permissions = permClaims });
        }

        if (!string.IsNullOrWhiteSpace(search))
            result = result.Where(x => x.User.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (x.User.Email ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(roleFilter))
            result = result.Where(x => x.Roles.Contains(roleFilter)).ToList();

        if (statusFilter == "active")
            result = result.Where(x => x.User.IsActive).ToList();
        else if (statusFilter == "inactive")
            result = result.Where(x => !x.User.IsActive).ToList();

        var allUsers = await _userManager.Users.ToListAsync();
        var stats = new UserStatsViewModel { Total = allUsers.Count, Active = allUsers.Count(u => u.IsActive), Inactive = allUsers.Count(u => !u.IsActive) };
        foreach (var u in allUsers)
        {
            var r = await _userManager.GetRolesAsync(u);
            if (r.Contains("Admin")) stats.Admins++; else stats.Employees++;
        }

        ViewBag.Stats = stats;
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        ViewBag.PermissionGroups = Permissions.Groups;
        ViewBag.Search = search;
        ViewBag.RoleFilter = roleFilter;
        ViewBag.StatusFilter = statusFilter;
        ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        ViewBag.PermissionGroups = Permissions.Groups;
        ViewBag.EmployeeDefaults = Permissions.EmployeeDefaults;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string fullName, string email, string password, string role, List<string>? permissions)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        { TempData["Error"] = "ط¬ظ…ظٹط¹ ط§ظ„ط­ظ‚ظˆظ„ ظ…ط·ظ„ظˆط¨ط©"; return RedirectToAction(nameof(Index)); }

        if (await _userManager.FindByEmailAsync(email) != null)
        { TempData["Error"] = "ط§ظ„ط¨ط±ظٹط¯ ط§ظ„ط¥ظ„ظƒطھط±ظˆظ†ظٹ ظ…ط³طھط®ط¯ظ… ط¨ط§ظ„ظپط¹ظ„"; return RedirectToAction(nameof(Index)); }

        var user = new ApplicationUser
        {
            UserName = email, Email = email, FullName = fullName,
            EmailConfirmed = true, IsActive = true, CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            TempData["Error"] = string.Join("طŒ ", result.Errors.Select(e => TranslateError(e.Code)));
            return RedirectToAction(nameof(Index));
        }

        if (!string.IsNullOrEmpty(role))
            await _userManager.AddToRoleAsync(user, role);

        // ط¥ط¶ط§ظپط© ط§ظ„طµظ„ط§ط­ظٹط§طھ
        if (role == "Employee")
        {
            var permsToAdd = permissions?.Any() == true ? permissions : Permissions.EmployeeDefaults.ToList();
            foreach (var p in permsToAdd)
                await _userManager.AddClaimAsync(user, new Claim("Permission", p));
        }

        TempData["Success"] = $"طھظ… ط¥ظ†ط´ط§ط، ط­ط³ط§ط¨ {fullName} ط¨ظ†ط¬ط§ط­";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, string fullName, string email, string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.FullName = fullName;
        if (user.Email != email)
        {
            user.Email = email; user.UserName = email;
            user.NormalizedEmail = email.ToUpper(); user.NormalizedUserName = email.ToUpper();
        }

        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!string.IsNullOrEmpty(role))
            await _userManager.AddToRoleAsync(user, role);

        TempData["Success"] = "طھظ… طھط¹ط¯ظٹظ„ ط¨ظٹط§ظ†ط§طھ ط§ظ„ظ…ط³طھط®ط¯ظ… ط¨ظ†ط¬ط§ط­";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePermissions(string id, List<string>? permissions)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // ط­ط°ظپ ظƒظ„ طµظ„ط§ط­ظٹط§طھ ط§ظ„ظ…ط³طھط®ط¯ظ… ط§ظ„ط­ط§ظ„ظٹط©
        var currentClaims = (await _userManager.GetClaimsAsync(user)).Where(c => c.Type == "Permission");
        foreach (var claim in currentClaims)
            await _userManager.RemoveClaimAsync(user, claim);

        // ط¥ط¶ط§ظپط© ط§ظ„طµظ„ط§ط­ظٹط§طھ ط§ظ„ط¬ط¯ظٹط¯ط©
        if (permissions?.Any() == true)
        {
            foreach (var p in permissions)
                await _userManager.AddClaimAsync(user, new Claim("Permission", p));
        }

        TempData["Success"] = $"طھظ… ط­ظپط¸ طµظ„ط§ط­ظٹط§طھ {user.FullName} ({permissions?.Count ?? 0} طµظ„ط§ط­ظٹط©)";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPermissions(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var perms = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

        return Json(new { isAdmin = roles.Contains("Admin"), permissions = perms, fullName = user.FullName });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == currentUserId) { TempData["Error"] = "ظ„ط§ ظٹظ…ظƒظ†ظƒ طھط¹ط·ظٹظ„ ط­ط³ط§ط¨ظƒ ط§ظ„ط®ط§طµ"; return RedirectToAction(nameof(Index)); }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = user.IsActive ? $"طھظ… طھظپط¹ظٹظ„ {user.FullName}" : $"طھظ… طھط¹ط·ظٹظ„ {user.FullName}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? $"طھظ… طھط؛ظٹظٹط± ظƒظ„ظ…ط© ظ…ط±ظˆط± {user.FullName}"
            : "ظپط´ظ„: " + string.Join("طŒ ", result.Errors.Select(e => TranslateError(e.Code)));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == currentUserId) { TempData["Error"] = "ظ„ط§ ظٹظ…ظƒظ†ظƒ ط­ط°ظپ ط­ط³ط§ط¨ظƒ ط§ظ„ط®ط§طµ"; return RedirectToAction(nameof(Index)); }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        TempData["Success"] = $"طھظ… ط­ط°ظپ ط§ظ„ظ…ط³طھط®ط¯ظ… {user.FullName}";
        return RedirectToAction(nameof(Index));
    }

    private static string TranslateError(string code) => code switch
    {
        "PasswordTooShort"                => "ظƒظ„ظ…ط© ط§ظ„ظ…ط±ظˆط± ظ‚طµظٹط±ط© ط¬ط¯ط§ظ‹ (6 ط£ط­ط±ظپ ظƒط­ط¯ ط£ط¯ظ†ظ‰)",
        "PasswordRequiresNonAlphanumeric" => "ظƒظ„ظ…ط© ط§ظ„ظ…ط±ظˆط± طھط­طھط§ط¬ ط±ظ…ط²ط§ظ‹ ط®ط§طµط§ظ‹",
        "PasswordRequiresDigit"           => "ظƒظ„ظ…ط© ط§ظ„ظ…ط±ظˆط± طھط­طھط§ط¬ ط±ظ‚ظ…ط§ظ‹",
        "PasswordRequiresUpper"           => "ظƒظ„ظ…ط© ط§ظ„ظ…ط±ظˆط± طھط­طھط§ط¬ ط­ط±ظپط§ظ‹ ظƒط¨ظٹط±ط§ظ‹",
        "DuplicateEmail"                  => "ط§ظ„ط¨ط±ظٹط¯ ط§ظ„ط¥ظ„ظƒطھط±ظˆظ†ظٹ ظ…ط³طھط®ط¯ظ… ط¨ط§ظ„ظپط¹ظ„",
        "DuplicateUserName"               => "ط§ط³ظ… ط§ظ„ظ…ط³طھط®ط¯ظ… ظ…ط³طھط®ط¯ظ… ط¨ط§ظ„ظپط¹ظ„",
        _                                 => code
    };
}

public class UserWithRolesViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public IList<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}

public class UserStatsViewModel
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public int Admins { get; set; }
    public int Employees { get; set; }
}


