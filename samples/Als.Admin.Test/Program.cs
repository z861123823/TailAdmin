using System.Text.Encodings.Web;
using System.Text.Unicode;
using Als.Admin.Test.Infrastructure;
using Als.Admin.Test.Services;
using Microsoft.Extensions.WebEncoders;
using Alys.TailAdmin.Rcl.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

// 允许中文按原文输出，避免页面源码出现大量数字实体
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

builder.Services.AddAlysAdminUi(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<AlsApiClient>();
builder.Services.AddSingleton<AlsAdminGateway>();

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", () => Results.Redirect("/Admin/Home/Index"));
app.MapControllers();
app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// <summary>供集成测试引用的入口标记。</summary>
public partial class Program
{
}
