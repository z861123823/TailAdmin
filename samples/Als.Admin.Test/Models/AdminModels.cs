using Als.Admin.Test.Services;

namespace Als.Admin.Test.Models;

/// <summary>商品列表页模型。</summary>
public sealed class GoodsListPageModel
{
    /// <summary>分类下拉选项。</summary>
    public List<GoodsTypeOption> Types { get; init; } = new();
}

/// <summary>商品弹窗表单模型。</summary>
public sealed class GoodsFormModel
{
    public int Id { get; set; }
    public string? GoodsNumber { get; set; }
    public string? GoodsName { get; set; }
    public decimal Price { get; set; }
    public int TypeId { get; set; }
    public string? Format { get; set; }
    public string? ProductDescription { get; set; }
    public string SpecData { get; set; } = "[]";
    public string SkuData { get; set; } = "{}";
    public string SkuTableList { get; set; } = "[]";
    public string ExtraImagesJson { get; set; } = "[]";
    public string? PicName { get; set; }
    public string? PicUrl { get; set; }
    public bool IsHot { get; set; }
    public bool IsRecommend { get; set; }
    public bool IsSift { get; set; }
    public List<GoodsTypeOption> Types { get; init; } = new();
    public string UploadUrl { get; set; } = string.Empty;
}

/// <summary>订单列表页模型。</summary>
public sealed class OrderListPageModel
{
    public string Currency { get; set; } = "¥";
    public string Today { get; set; } = "0.00";
    public string Yesterday { get; set; } = "0.00";
    public string Month { get; set; } = "0.00";
    public string LastMonth { get; set; } = "0.00";
    public string Year { get; set; } = "0.00";
}

/// <summary>订单编辑弹窗模型。</summary>
public sealed class OrderFormModel
{
    public int Id { get; set; }
    public string? CartOrders { get; set; }
    public string? PayId { get; set; }
    public string? PayText { get; set; }
    public string? BuyTimes { get; set; }
    public string? BuyerName { get; set; }
    public string? BuyerPhone { get; set; }
    public string? ReceiveAddress { get; set; }
    public string? TotalText { get; set; }
    public int PayStatus { get; set; }
    public int PayType { get; set; }
    public string? ShipmentState { get; set; }
    public string? Logistic { get; set; }
    public string? LogisticsId { get; set; }
    public string? Remark { get; set; }
}

/// <summary>订单详情中的商品行。</summary>
public sealed record OrderGoodsRow(string? GoodsName, string? PicUrl, decimal Price, int BuyQty, decimal Total);

/// <summary>订单详情模型。</summary>
public sealed class OrderDetailModel
{
    public OrderFormModel Order { get; set; } = new();
    public List<OrderGoodsRow> Goods { get; set; } = new();
}

/// <summary>商品分类表单模型。</summary>
public sealed class GoodsTypeFormModel
{
    public int TypeId { get; set; }
    public string? TypeName { get; set; }
    public int ParentId { get; set; }
    public List<GoodsTypeOption> Types { get; init; } = new();
}

/// <summary>文章分类表单模型。</summary>
public sealed class ArticleTypeFormModel
{
    public int Id { get; set; }
    public string? TypeName { get; set; }
}

/// <summary>文章表单模型。</summary>
public sealed class ArticleFormModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int TypeId { get; set; }
    public string? Contents { get; set; }
    public List<ArticleTypeOption> Types { get; init; } = new();
}

/// <summary>轮播图表单模型。</summary>
public sealed class SlideFormModel
{
    public int Id { get; set; }
    public string? PicName { get; set; }
    public string? PicUrl { get; set; }
    public string? PreviewUrl { get; set; }
}

/// <summary>首页设置页模型。</summary>
public sealed class HomeSectionsPageModel
{
    public string Mode { get; set; } = "flags";
    public List<int> CategoryIds { get; set; } = new();
    public List<GoodsTypeOption> Types { get; set; } = new();
}

/// <summary>仪表盘商品行。</summary>
public sealed record DashboardGoodsRow(
    int Id,
    string GoodsName,
    string TypeName,
    string PriceText,
    string? PicUrl,
    int SalesVolume,
    string AddTime);

/// <summary>仪表盘模型。</summary>
public sealed class DashboardModel
{
    public int GoodsCount { get; set; }
    public int TypeCount { get; set; }
    public int OrderCount { get; set; }
    public int SoldCount { get; set; }
    public int ArticleCount { get; set; }
    public string Currency { get; set; } = "¥";
    public string Today { get; set; } = "0.00";
    public string Yesterday { get; set; } = "0.00";
    public string Month { get; set; } = "0.00";
    public string LastMonth { get; set; } = "0.00";
    public string Year { get; set; } = "0.00";
    public List<DashboardGoodsRow> RecentGoods { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public string MallBaseUrl { get; set; } = string.Empty;
}
