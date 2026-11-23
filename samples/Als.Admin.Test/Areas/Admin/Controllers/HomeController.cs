using System.Text.Json.Nodes;
using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>仪表盘：聚合商城后台已有的统计接口。</summary>
[Area("Admin")]
public sealed class HomeController : AlsControllerBase
{
    /// <summary>构造函数。</summary>
    public HomeController(AlsApiClient api, ILogger<HomeController> logger)
        : base(api, logger)
    {
    }

    /// <summary>仪表盘首页。</summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "儀表盤";
        ViewData["Subtitle"] = "即時聚合商城後台商品、訂單、銷售與內容數據";
        ViewData["MenuKey"] = "dashboard";
        ViewData["Breadcrumb"] = new[] { "後台", "儀表盤" };

        var model = new DashboardModel { MallBaseUrl = Api.BaseUrl };

        var goodsTask = Api.GetJsonAsync("/Admin/Goods/List", new Dictionary<string, string?>
        {
            ["page"] = "1",
            ["limit"] = "5"
        }, cancellationToken);
        var typesTask = Api.GetJsonAsync("/Admin/GoodsType/List", cancellationToken: cancellationToken);
        var ordersTask = Api.GetJsonAsync("/Admin/Order/List", new Dictionary<string, string?>
        {
            ["page"] = "1",
            ["limit"] = "1"
        }, cancellationToken);
        var soldTask = Api.GetJsonAsync("/Admin/GoodsSold/List", new Dictionary<string, string?>
        {
            ["page"] = "1",
            ["limit"] = "1"
        }, cancellationToken);
        var articlesTask = Api.GetJsonAsync("/Admin/Article/List", cancellationToken: cancellationToken);
        var payTask = Api.GetJsonAsync("/Admin/Order/GetPayTotal", cancellationToken: cancellationToken);

        try
        {
            await Task.WhenAll(goodsTask, typesTask, ordersTask, soldTask, articlesTask, payTask);
        }
        catch (Exception exception)
        {
            model.Warnings.Add("商城接口调用失败：" + exception.Message);
            return View(model);
        }

        var goods = await goodsTask;
        var types = await typesTask;
        var orders = await ordersTask;
        var sold = await soldTask;
        var articles = await articlesTask;
        var pay = await payTask;

        model.GoodsCount = AlsJson.Int(goods, "count");
        model.TypeCount = AlsJson.Int(types, "count");
        model.OrderCount = AlsJson.Int(orders, "count");
        model.SoldCount = AlsJson.Int(sold, "count");
        model.ArticleCount = AlsJson.Int(articles, "count");

        if (goods is null || types is null || orders is null)
        {
            model.Warnings.Add("商城後台部分統計接口未返回數據，請確認商城服務已啟動且後台登入校驗已關閉。");
        }

        if (pay is not null)
        {
            model.Currency = AlsJson.Str(pay, "Currency") ?? model.Currency;
            model.Today = AlsJson.Str(pay, "Today") ?? model.Today;
            model.Yesterday = AlsJson.Str(pay, "Yesterday") ?? model.Yesterday;
            model.Month = AlsJson.Str(pay, "Month") ?? model.Month;
            model.LastMonth = AlsJson.Str(pay, "LastMonth") ?? model.LastMonth;
            model.Year = AlsJson.Str(pay, "Year") ?? model.Year;
        }

        var rows = AlsJson.Array(goods, "data");
        if (rows is not null)
        {
            foreach (var row in rows)
            {
                model.RecentGoods.Add(new DashboardGoodsRow(
                    AlsJson.Int(row, "Id"),
                    AlsJson.Str(row, "GoodsName") ?? string.Empty,
                    AlsJson.Str(row, "TypeName") ?? "未分類",
                    AlsJson.Str(row, "PriceText") ?? AlsJson.Str(row, "Price") ?? "0",
                    AlsJson.AssetUrl(AlsJson.Str(row, "PicUrls")),
                    AlsJson.Int(row, "SalesVolume"),
                    AlsJson.Str(row, "AddTime") ?? string.Empty));
            }
        }

        return View(model);
    }
}
