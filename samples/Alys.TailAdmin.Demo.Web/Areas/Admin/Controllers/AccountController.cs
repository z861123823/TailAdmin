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
        ViewData["Message"] = TempData["AuthMessage"] as string;
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

    /// <summary>注册页。</summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignUp()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        ViewData["Title"] = "註冊";
        return View();
    }

    /// <summary>提交注册（演示：写入内存账号列表）。</summary>
    /// <param name="userName">账号。</param>
    /// <param name="email">邮箱。</param>
    /// <param name="password">密码。</param>
    /// <param name="confirmPassword">确认密码。</param>
    [AllowAnonymous]
    [HttpPost]
    public IActionResult SignUp(string? userName, string? email, string? password, string? confirmPassword)
    {
        ViewData["Title"] = "註冊";
        ViewData["UserName"] = userName;
        ViewData["Email"] = email;

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewData["Error"] = "請填寫完整的註冊資料";
            return View();
        }

        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
        {
            ViewData["Error"] = "兩次輸入的密碼不一致";
            return View();
        }

        if (_options.Value.Accounts.Any(item => string.Equals(item.UserName, userName, StringComparison.OrdinalIgnoreCase)))
        {
            ViewData["Error"] = "該帳號已被註冊";
            return View();
        }

        _options.Value.Accounts.Add(new DemoAccount
        {
            UserName = userName,
            Password = password,
            DisplayName = userName,
            Permissions = new List<string> { "dashboard" }
        });
        TempData["AuthMessage"] = "註冊成功，請使用新帳號登入";
        return RedirectToAction(nameof(Login));
    }

    /// <summary>忘记密码页。</summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword()
    {
        ViewData["Title"] = "重設密碼";
        return View();
    }

    /// <summary>提交找回密码（演示：仅提示，不发送邮件）。</summary>
    /// <param name="email">邮箱。</param>
    [AllowAnonymous]
    [HttpPost]
    public IActionResult ResetPassword(string? email)
    {
        ViewData["Title"] = "重設密碼";
        ViewData["Email"] = email;

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            ViewData["Error"] = "請輸入有效的郵箱地址";
            return View();
        }

        TempData["AuthMessage"] = "重設連結已發送至郵箱（示例環境不會實際發送）";
        return RedirectToAction(nameof(Login));
    }

    /// <summary>两步验证页。</summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult TwoStepVerification()
    {
        ViewData["Title"] = "兩步驗證";
        return View();
    }

    /// <summary>锁屏页。</summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult LockScreen()
    {
        ViewData["Title"] = "鎖屏";
        return View();
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
