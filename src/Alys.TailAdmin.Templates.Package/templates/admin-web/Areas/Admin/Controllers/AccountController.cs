using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlysAdminHost.Controllers;

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

    /// <summary>提交注册（模板演示：仅校验表单，接入项目时写入真实用户表）。</summary>
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

        // TODO: 接入实际用户体系后在此保存账号
        TempData["AuthMessage"] = "註冊資料已提交，接入用戶體系後即可登入（模板演示）";
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

    /// <summary>提交找回密码（模板演示：仅校验邮箱，接入项目时发送邮件）。</summary>
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

        // TODO: 接入邮件服务后在此发送重设链接
        TempData["AuthMessage"] = "重設連結已發送（模板演示不會實際發送郵件）";
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
