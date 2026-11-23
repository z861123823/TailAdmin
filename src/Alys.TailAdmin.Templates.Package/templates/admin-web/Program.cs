using System.Text.Encodings.Web;
using Microsoft.Extensions.WebEncoders;
#if (authCookie)
using Microsoft.AspNetCore.Authentication.Cookies;
#else
using Microsoft.AspNetCore.Authorization;
#endif
using AlysAdminHost.Infrastructure;
#if (sampleEnabled)
using Alys.TailAdmin.Rcl.Options;
#endif
using Alys.TailAdmin.Rcl.Extensions;
using Alys.TailAdmin.Rcl.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

// 允许中文按原文输出（仍会对 < > & " 等 HTML 敏感字符编码），避免页面源码出现大量数字实体
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(System.Text.Unicode.UnicodeRanges.All));

builder.Services.AddAlysAdminUi(builder.Configuration);

#if (sampleEnabled)
// 示例菜单仅在 --sample true 时注册，避免指向不存在的演示页
builder.Services.Configure<AdminUiOptions>(options => options.Menu.Items.Add(new AdminMenuItem
{
    Key = "goods",
    Title = "商品管理",
    Icon = "goods",
    Area = "Admin",
    Controller = "Goods",
    Action = "Index",
    Permission = "goods"
}));
#endif

#if (authCookie)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentAdmin, TemplateCurrentAdmin>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.LogoutPath = "/Admin/Account/Logout";
        options.Cookie.Name = "AlysAdminHost";
        options.Events.OnRedirectToLogin = context =>
        {
            if (string.Equals(context.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json; charset=utf-8";
                return context.Response.WriteAsJsonAsync(new { Success = false, Message = "請重新登入" });
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });
#else
builder.Services.AddSingleton<ICurrentAdmin, GuestCurrentAdmin>();
builder.Services.AddAuthorization(options =>
{
    // --auth none：模板仍保留 [Authorize] 标注，这里放行默认策略，避免无认证方案时 500。
    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAssertion(_ => true).Build();
});
#endif

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
#if (authCookie)
app.UseAuthentication();
#endif
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/Admin/Home/Index"));
app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// <summary>供集成测试引用的入口标记。</summary>
public partial class Program
{
}
