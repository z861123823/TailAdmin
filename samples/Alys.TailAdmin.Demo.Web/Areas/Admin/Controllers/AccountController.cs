using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Alys.TailAdmin.Demo.Web.Infrastructure;

namespace Alys.TailAdmin.Demo.Web.Areas.Admin.Controllers;

/// <summary>登录与登出（演示用 Cookie 鉴权）。</summary>
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public sealed class AccountController : Controller
{
    private readonly IOptions<DemoOptions> _options;

    /// <summary>初始化账号控制器。</summary>
    /// <param name="options">演示账号配置。</param>
    public AccountController(IOptions<DemoOptions> options)
    {
        _options = options;
    }

    /// <summary>登录页。</summary>
    /// <param name="returnUrl">登录后跳转地址。</param>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        ViewData["Title"] = "登錄";
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>提交登录。</summary>
    /// <param name="userName">账号。</param>
    /// <param name="password">密码。</param>
    /// <param name="returnUrl">登录后跳转地址。</param>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string userName, string password, string? returnUrl)
    {
        var account = _options.Value.Accounts.FirstOrDefault(item =>
            string.Equals(item.UserName, userName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(item.Password, password, StringComparison.Ordinal));

        if (account is null)
        {
            ViewData["Title"] = "登錄";
            ViewData["Error"] = "帳號或密碼錯誤";
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["UserName"] = userName;
            return View();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, account.UserName),
            new(DemoClaims.DisplayName, account.DisplayName)
        };
        claims.AddRange(account.Permissions.Select(permission => new Claim(DemoClaims.Permission, permission)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home", new { area = "Admin" });
    }

    /// <summary>登出。</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
