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
        Key = "layouts",
        Title = "佈局",
        Icon = "components",
        Permission = "layouts",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "layout-default", Title = "預設佈局", Area = "Admin", Controller = "Layouts", Action = "Default", Permission = "layouts" },
            new() { Key = "layout-collapsed", Title = "側欄收合", Area = "Admin", Controller = "Layouts", Action = "Collapsed", Permission = "layouts" },
            new() { Key = "layout-hidden-sidebar", Title = "無側欄", Area = "Admin", Controller = "Layouts", Action = "HiddenSidebar", Permission = "layouts" },
            new() { Key = "layout-hidden-header", Title = "無頂欄", Area = "Admin", Controller = "Layouts", Action = "HiddenHeader", Permission = "layouts" },
            new() { Key = "layout-full", Title = "全寬內容", Area = "Admin", Controller = "Layouts", Action = "FullWidth", Permission = "layouts" },
            new() { Key = "layout-narrow", Title = "窄內容", Area = "Admin", Controller = "Layouts", Action = "Narrow", Permission = "layouts" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "auth",
        Title = "認證",
        Icon = "user",
        Permission = "auth",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "auth-2fa", Title = "兩步驗證", Area = "Admin", Controller = "Account", Action = "TwoStepVerification", Permission = "auth" },
            new() { Key = "auth-lock", Title = "鎖屏", Area = "Admin", Controller = "Account", Action = "LockScreen", Permission = "auth" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "ai",
        Title = "AI",
        Icon = "components",
        Permission = "ai",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "ai-chat", Title = "AI 助手", Area = "Admin", Controller = "Ai", Action = "Chat", Permission = "ai" },
            new() { Key = "ai-text", Title = "文本生成", Area = "Admin", Controller = "Ai", Action = "TextGenerator", Permission = "ai" },
            new() { Key = "ai-image", Title = "圖片生成", Area = "Admin", Controller = "Ai", Action = "ImageGenerator", Permission = "ai" },
            new() { Key = "ai-code", Title = "代碼生成", Area = "Admin", Controller = "Ai", Action = "CodeGenerator", Permission = "ai" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "ai-settings",
        Title = "AI 設定",
        Icon = "settings",
        Permission = "ai-settings",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "ai-models", Title = "模型設定", Area = "Admin", Controller = "AiSettings", Action = "Models", Permission = "ai-settings" },
            new() { Key = "ai-api-keys", Title = "API 金鑰", Area = "Admin", Controller = "AiSettings", Action = "ApiKeys", Permission = "ai-settings" },
            new() { Key = "ai-usage", Title = "用量統計", Area = "Admin", Controller = "AiSettings", Action = "Usage", Permission = "ai-settings" },
            new() { Key = "ai-prompts", Title = "提示詞庫", Area = "Admin", Controller = "AiSettings", Action = "Prompts", Permission = "ai-settings" },
            new() { Key = "ai-safety", Title = "安全與限制", Area = "Admin", Controller = "AiSettings", Action = "Safety", Permission = "ai-settings" },
            new() { Key = "ai-history", Title = "對話記錄", Area = "Admin", Controller = "AiSettings", Action = "History", Permission = "ai-settings" }
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
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "calendar",
        Title = "日曆",
        Icon = "calendar",
        Area = "Admin",
        Controller = "Calendar",
        Action = "Index",
        Permission = "calendar"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "profile",
        Title = "個人資料",
        Icon = "user",
        Area = "Admin",
        Controller = "Profile",
        Action = "Index",
        Permission = "profile"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "empty-state",
        Title = "空狀態",
        Icon = "empty",
        Area = "Admin",
        Controller = "EmptyState",
        Action = "Index",
        Permission = "empty-state"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "onboarding",
        Title = "引導流程",
        Icon = "check",
        Area = "Admin",
        Controller = "Onboarding",
        Action = "Index",
        Permission = "onboarding"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "ecommerce",
        Title = "電子商務",
        Icon = "goods",
        Permission = "ecommerce",
        Children = new List<AdminMenuItem>
        {
            new() { Key = "ecommerce-products", Title = "商品列表", Area = "Admin", Controller = "Ecommerce", Action = "Products", Permission = "ecommerce" },
            new() { Key = "ecommerce-add", Title = "新增商品", Area = "Admin", Controller = "Ecommerce", Action = "AddProduct", Permission = "ecommerce" },
            new() { Key = "ecommerce-billing", Title = "帳單", Area = "Admin", Controller = "Ecommerce", Action = "Billing", Permission = "ecommerce" },
            new() { Key = "ecommerce-invoices", Title = "發票", Area = "Admin", Controller = "Ecommerce", Action = "Invoices", Permission = "ecommerce" }
        }
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "support",
        Title = "客服工單",
        Icon = "bell",
        Area = "Admin",
        Controller = "Support",
        Action = "Tickets",
        Permission = "support"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "email",
        Title = "郵件",
        Icon = "mail",
        Area = "Admin",
        Controller = "Email",
        Action = "Inbox",
        Permission = "email"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "chat",
        Title = "聊天",
        Icon = "chat",
        Area = "Admin",
        Controller = "Chat",
        Action = "Index",
        Permission = "chat"
    });
    options.Menu.Items.Add(new AdminMenuItem
    {
        Key = "tasks",
        Title = "任務看板",
        Icon = "check",
        Area = "Admin",
        Controller = "Tasks",
        Action = "Kanban",
        Permission = "tasks"
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
