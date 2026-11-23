using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TailAdminHost.Controllers;

/// <summary>登录与登出（模板内置演示账号，接入真实用户体系时替换登录校验）。</summary>
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public sealed class AccountController : Controller
{
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
        // 模板演示账号，接入项目时请替换为真实用户校验
        if (!string.Equals(userName, "admin", StringComparison.OrdinalIgnoreCase) || password != "123456")
        {
            ViewData["Title"] = "登錄";
            ViewData["Error"] = "帳號或密碼錯誤";
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["UserName"] = userName;
            return View();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new("display_name", "管理員"),
            new("perm", "*")
        };
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
