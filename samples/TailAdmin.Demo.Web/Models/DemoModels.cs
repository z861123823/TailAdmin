using System.ComponentModel.DataAnnotations;

namespace TailAdmin.Demo.Web.Models;

/// <summary>商品实体（示例内存数据）。</summary>
public sealed class GoodsItem
{
    public int Id { get; set; }
    public string GoodsName { get; set; } = string.Empty;
    public string? PicUrl { get; set; }
    public decimal Price { get; set; }
    public int TypeId { get; set; }
    public List<string> Tags { get; set; } = new();
    public int SalesVolume { get; set; }
    public bool IsHot { get; set; }
    public DateTime AddTime { get; set; }
    public string? Remark { get; set; }
}

/// <summary>商品分类实体。</summary>
public sealed class GoodsTypeItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sort { get; set; }
    public bool Enabled { get; set; } = true;
}

/// <summary>商品列表行。</summary>
public sealed class GoodsListItem
{
    public int Id { get; set; }
    public string GoodsName { get; set; } = string.Empty;
    public string? PicUrl { get; set; }
    public decimal Price { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string TagsText { get; set; } = string.Empty;
    public int SalesVolume { get; set; }
    public bool IsHot { get; set; }
    public string AddTime { get; set; } = string.Empty;
}

/// <summary>商品新增/编辑模型。</summary>
public sealed class GoodsEditModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "商品名稱不能為空")]
    [StringLength(60, ErrorMessage = "商品名稱不能超過 60 個字")]
    public string GoodsName { get; set; } = string.Empty;

    public string? PicUrl { get; set; }

    [Range(0, 9999999, ErrorMessage = "價格必須在 0 到 9999999 之間")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "請選擇商品分類")]
    public int TypeId { get; set; }

    public List<string> Tags { get; set; } = new();

    [Range(0, int.MaxValue, ErrorMessage = "銷量不能為負數")]
    public int SalesVolume { get; set; }

    public bool IsHot { get; set; }

    public string? AddTime { get; set; }

    [StringLength(200, ErrorMessage = "備註不能超過 200 個字")]
    public string? Remark { get; set; }
}

/// <summary>分类新增/编辑模型。</summary>
public sealed class GoodsTypeEditModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "分類名稱不能為空")]
    [StringLength(30, ErrorMessage = "分類名稱不能超過 30 個字")]
    public string Name { get; set; } = string.Empty;

    [Range(0, 9999, ErrorMessage = "排序必須在 0 到 9999 之間")]
    public int Sort { get; set; }

    public bool Enabled { get; set; } = true;
}

/// <summary>分类列表行。</summary>
public sealed class GoodsTypeListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sort { get; set; }
    public bool Enabled { get; set; }
    public int GoodsCount { get; set; }
}

/// <summary>仪表盘视图模型。</summary>
public sealed class DashboardViewModel
{
    public int GoodsCount { get; set; }
    public decimal SalesAmount { get; set; }
    public int TypeCount { get; set; }
    public int HotCount { get; set; }
    public IReadOnlyList<GoodsListItem> RecentGoods { get; set; } = Array.Empty<GoodsListItem>();
}

/// <summary>统一写操作响应。</summary>
public sealed class ApiResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public object? Errors { get; set; }
}

/// <summary>统一响应工厂。</summary>
public static class ApiResults
{
    /// <summary>列表响应（保持 layui 形状）。</summary>
    public static object Table(object data, int count)
        => new Dictionary<string, object?> { ["code"] = 0, ["msg"] = string.Empty, ["count"] = count, ["data"] = data };

    /// <summary>成功响应。</summary>
    public static ApiResult Ok(string message = "操作成功", object? data = null)
        => new() { Success = true, Message = message, Data = data };

    /// <summary>失败响应。</summary>
    public static ApiResult Fail(string message, object? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
