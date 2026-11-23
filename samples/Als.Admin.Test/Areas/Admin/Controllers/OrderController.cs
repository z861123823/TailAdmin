using System.Globalization;
using System.Text.Json.Nodes;
using Als.Admin.Test.Models;
using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Areas.Admin.Controllers;

/// <summary>订单管理：列表、详情、发货信息维护、退款与删除。</summary>
[Area("Admin")]
public sealed class OrderController : AlsControllerBase
{
    /// <summary>构造函数。</summary>
    public OrderController(AlsApiClient api, ILogger<OrderController> logger) : base(api, logger)
    {
    }

    /// <summary>订单列表页。</summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "訂單管理";
        ViewData["Subtitle"] = "對接商城 /Admin/Order 接口，可查看明細、維護發貨資訊與發起退款";
        ViewData["MenuKey"] = "order";
        ViewData["Breadcrumb"] = new[] { "後台", "交易中心", "訂單管理" };

        var model = new OrderListPageModel();
        var pay = await Api.GetJsonAsync("/Admin/Order/GetPayTotal", cancellationToken: cancellationToken);
        if (pay is not null)
        {
            model.Currency = AlsJson.Str(pay, "Currency") ?? model.Currency;
            model.Today = AlsJson.Str(pay, "Today") ?? model.Today;
            model.Yesterday = AlsJson.Str(pay, "Yesterday") ?? model.Yesterday;
            model.Month = AlsJson.Str(pay, "Month") ?? model.Month;
            model.LastMonth = AlsJson.Str(pay, "LastMonth") ?? model.LastMonth;
            model.Year = AlsJson.Str(pay, "Year") ?? model.Year;
        }

        return View(model);
    }

    /// <summary>订单列表数据源（补充付款状态文案，便于表格直接展示）。</summary>
    [HttpGet]
    public async Task<IActionResult> List(int page = 1, int limit = 15, CancellationToken cancellationToken = default)
    {
        try
        {
            var node = await Api.GetJsonAsync("/Admin/Order/List", new Dictionary<string, string?>
            {
                ["page"] = Math.Max(1, page).ToString(CultureInfo.InvariantCulture),
                ["limit"] = Math.Clamp(limit, 1, 200).ToString(CultureInfo.InvariantCulture)
            }, cancellationToken);

            if (AlsJson.Array(node, "data") is { } rows)
            {
                foreach (var row in rows)
                {
                    if (row is JsonObject obj)
                    {
                        obj["PayStatusText"] = PayStatusText(AlsJson.Int(row, "PayStatus"));
                    }
                }
            }

            AlsJson.PrefixAssetUrls(node);
            return Content(AlsJson.ToJson(node), "application/json; charset=utf-8");
        }
        catch (Exception exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                code = 1,
                msg = "商城接口调用失败：" + exception.Message,
                count = 0,
                data = Array.Empty<object>()
            });
        }
    }

    /// <summary>已支付金额汇总。</summary>
    [HttpGet]
    public Task<IActionResult> PayTotal(CancellationToken cancellationToken)
        => ForwardJsonAsync("/Admin/Order/GetPayTotal", null, cancellationToken);

    /// <summary>发货信息编辑弹窗。</summary>
    [HttpGet]
    public async Task<IActionResult> EditForm(int id, CancellationToken cancellationToken)
    {
        var detail = await LoadDetailAsync(id, cancellationToken);
        return detail is null
            ? Content("<div class=\"p-4 text-theme-sm text-red-500\">商城未返回該訂單。</div>", "text/html; charset=utf-8")
            : PartialView("_Form", detail.Value.Order);
    }

    /// <summary>订单详情弹窗。</summary>
    [HttpGet]
    public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
    {
        var detail = await LoadDetailAsync(id, cancellationToken);
        if (detail is null)
        {
            return Content("<div class=\"p-4 text-theme-sm text-red-500\">商城未返回該訂單。</div>", "text/html; charset=utf-8");
        }

        return PartialView("_Detail", new OrderDetailModel
        {
            Order = detail.Value.Order,
            Goods = detail.Value.Goods
        });
    }

    /// <summary>保存发货信息。</summary>
    [HttpPost]
    public Task<IActionResult> Save(
        int id,
        string? shipmentState,
        string? logistic,
        string? logisticsId,
        string? remark,
        CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Order/Save", new[]
        {
            Pair("id", id),
            Pair("shipmentState", shipmentState),
            Pair("logistic", logistic),
            Pair("logisticsId", logisticsId),
            Pair("remark", remark)
        }, cancellationToken);

    /// <summary>发起退款（参数为订单编号）。</summary>
    [HttpPost]
    public Task<IActionResult> Refund(string? id, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Order/Refund", new[] { Pair("id", id) }, cancellationToken);

    /// <summary>删除订单。</summary>
    [HttpPost]
    public Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        => ForwardPostAsync("/Admin/Order/Delete", new[] { Pair("id", id) }, cancellationToken);

    private async Task<(OrderFormModel Order, List<OrderGoodsRow> Goods)?> LoadDetailAsync(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return null;
        }

        var node = await Api.GetJsonAsync("/Admin/Order/Details", new Dictionary<string, string?>
        {
            ["id"] = id.ToString(CultureInfo.InvariantCulture)
        }, cancellationToken);

        var order = AlsJson.Object(node, "Order");
        if (order is null)
        {
            return null;
        }

        var total = AlsJson.Decimal(order, "Total");
        var model = new OrderFormModel
        {
            Id = id,
            CartOrders = AlsJson.Str(order, "CartOrders"),
            PayId = AlsJson.Str(order, "PayId"),
            PayText = PayStatusText(AlsJson.Int(order, "PayStatus")),
            BuyTimes = AlsJson.Str(order, "BuyTimes"),
            BuyerName = AlsJson.Str(order, "BuyerName"),
            BuyerPhone = AlsJson.Str(order, "BuyerPhone"),
            ReceiveAddress = AlsJson.Str(order, "ReceiveAddress"),
            TotalText = total.ToString("N2", CultureInfo.InvariantCulture),
            PayStatus = AlsJson.Int(order, "PayStatus"),
            PayType = AlsJson.Int(order, "PayType"),
            ShipmentState = AlsJson.Str(order, "ShipmentState"),
            Logistic = AlsJson.Str(order, "Logistic"),
            LogisticsId = AlsJson.Str(order, "LogisticsId"),
            Remark = AlsJson.Str(order, "Remark")
        };

        var goods = new List<OrderGoodsRow>();
        var goodsRows = AlsJson.Array(node, "GoodsList");
        if (goodsRows is not null)
        {
            foreach (var row in goodsRows)
            {
                var price = AlsJson.Decimal(row, "Price");
                var qty = AlsJson.Int(row, "BuyQty");
                goods.Add(new OrderGoodsRow(
                    AlsJson.Str(row, "GoodsName"),
                    AlsJson.AssetUrl(AlsJson.Str(row, "PicUrls")),
                    price,
                    qty,
                    AlsJson.Decimal(row, "Total", price * qty)));
            }
        }

        return (model, goods);
    }

    private static string PayStatusText(int status) => status switch
    {
        1 => "已付款",
        2 => "已退款",
        3 => "取消付款",
        4 => "交易失敗",
        5 => "退款中",
        _ => "未付款"
    };

    private static KeyValuePair<string, string?> Pair(string key, object? value)
        => new(key, value?.ToString());
}
