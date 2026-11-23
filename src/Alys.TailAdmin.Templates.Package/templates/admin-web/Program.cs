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
builder.Services.Configure<AdminUiOptions>(options =>
{
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "goods",
        Title = "商品管理",
        Icon = "goods",
        Area = "Admin",
        Controller = "Goods",
        Action = "Index",
        Permission = "goods"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "ui-elements",
        Title = "UI 元件",
        Icon = "components",
        Permission = "ui-elements",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "ui-alerts", Title = "提示", Area = "Admin", Controller = "UiElements", Action = "Alerts", Permission = "ui-elements" },
            new() { Key = "ui-avatars", Title = "頭像", Area = "Admin", Controller = "UiElements", Action = "Avatars", Permission = "ui-elements" },
            new() { Key = "ui-badges", Title = "徽章", Area = "Admin", Controller = "UiElements", Action = "Badges", Permission = "ui-elements" },
            new() { Key = "ui-breadcrumb", Title = "麵包屑", Area = "Admin", Controller = "UiElements", Action = "Breadcrumb", Permission = "ui-elements" },
            new() { Key = "ui-buttons", Title = "按鈕", Area = "Admin", Controller = "UiElements", Action = "Buttons", Permission = "ui-elements" },
            new() { Key = "ui-button-group", Title = "按鈕組", Area = "Admin", Controller = "UiElements", Action = "ButtonGroup", Permission = "ui-elements" },
            new() { Key = "ui-cards", Title = "卡片", Area = "Admin", Controller = "UiElements", Action = "Cards", Permission = "ui-elements" },
            new() { Key = "ui-carousel", Title = "輪播", Area = "Admin", Controller = "UiElements", Action = "Carousel", Permission = "ui-elements" },
            new() { Key = "ui-dropdowns", Title = "下拉選單", Area = "Admin", Controller = "UiElements", Action = "Dropdowns", Permission = "ui-elements" },
            new() { Key = "ui-images", Title = "圖片", Area = "Admin", Controller = "UiElements", Action = "Images", Permission = "ui-elements" },
            new() { Key = "ui-links", Title = "連結", Area = "Admin", Controller = "UiElements", Action = "Links", Permission = "ui-elements" },
            new() { Key = "ui-lists", Title = "列表", Area = "Admin", Controller = "UiElements", Action = "Lists", Permission = "ui-elements" },
            new() { Key = "ui-modals", Title = "彈窗", Area = "Admin", Controller = "UiElements", Action = "Modals", Permission = "ui-elements" },
            new() { Key = "ui-pagination", Title = "分頁", Area = "Admin", Controller = "UiElements", Action = "Pagination", Permission = "ui-elements" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "pages",
        Title = "頁面",
        Icon = "menu",
        Permission = "pages",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "page-blank", Title = "空白頁", Area = "Admin", Controller = "Pages", Action = "Blank", Permission = "pages" },
            new() { Key = "page-404", Title = "404 頁面", Area = "Admin", Controller = "Pages", Action = "Error404", Permission = "pages" },
            new() { Key = "page-coming-soon", Title = "即將上線", Area = "Admin", Controller = "Pages", Action = "ComingSoon", Permission = "pages" },
            new() { Key = "page-maintenance", Title = "系統維護", Area = "Admin", Controller = "Pages", Action = "Maintenance", Permission = "pages" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "forms",
        Title = "表單",
        Icon = "pencil",
        Permission = "forms",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "form-elements", Title = "表單元素", Area = "Admin", Controller = "Forms", Action = "Elements", Permission = "forms" },
            new() { Key = "form-layout", Title = "表單佈局", Area = "Admin", Controller = "Forms", Action = "Layout", Permission = "forms" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "tables",
        Title = "表格",
        Icon = "order",
        Permission = "tables",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "table-basic", Title = "基礎表格", Area = "Admin", Controller = "Tables", Action = "Basic", Permission = "tables" },
            new() { Key = "table-data", Title = "數據表格", Area = "Admin", Controller = "Tables", Action = "Data", Permission = "tables" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "charts",
        Title = "圖表",
        Icon = "chart",
        Permission = "charts",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "chart-line", Title = "折線圖", Area = "Admin", Controller = "Charts", Action = "Line", Permission = "charts" },
            new() { Key = "chart-bar", Title = "長條圖", Area = "Admin", Controller = "Charts", Action = "Bar", Permission = "charts" },
            new() { Key = "chart-pie", Title = "圓餅圖", Area = "Admin", Controller = "Charts", Action = "Pie", Permission = "charts" }
        }
    });
});
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
