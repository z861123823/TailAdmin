using Microsoft.AspNetCore.Mvc;

namespace TailAdmin.Templates.Rcl.ViewComponents;

/// <summary>仪表盘统计卡片视图组件。</summary>
public sealed class StatCardViewComponent : ViewComponent
{
    /// <summary>渲染统计卡片。</summary>
    /// <param name="title">标题。</param>
    /// <param name="value">主数值。</param>
    /// <param name="icon">图标名。</param>
    /// <param name="variant">颜色语义：primary / success / warning / info。</param>
    /// <param name="hint">辅助说明。</param>
    /// <returns>统计卡片视图。</returns>
    public IViewComponentResult Invoke(string title, string value, string icon = "chart", string variant = "primary", string? hint = null)
    {
        var model = new StatCardViewModel
        {
            Title = title,
            Value = value,
            Icon = icon,
            Variant = variant,
            Hint = hint
        };

        return View(model);
    }
}

/// <summary>统计卡片视图模型。</summary>
public sealed class StatCardViewModel
{
    /// <summary>标题。</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>主数值。</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>图标名。</summary>
    public string Icon { get; set; } = "chart";

    /// <summary>颜色语义。</summary>
    public string Variant { get; set; } = "primary";

    /// <summary>辅助说明。</summary>
    public string? Hint { get; set; }
}
