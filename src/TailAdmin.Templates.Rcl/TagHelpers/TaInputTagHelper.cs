using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TailAdmin.Templates.Rcl.TagHelpers;

/// <summary>daisyUI 输入框：&lt;ta-input asp-for="Name" label="名稱" /&gt;。</summary>
[HtmlTargetElement("ta-input")]
public sealed class TaInputTagHelper : TaFieldTagHelperBase
{
    /// <summary>input type。</summary>
    public string Type { get; set; } = "text";

    /// <summary>占位文本。</summary>
    public string? Placeholder { get; set; }

    /// <summary>数字步长。</summary>
    public string? Step { get; set; }

    /// <summary>最小值。</summary>
    public string? Min { get; set; }

    /// <summary>最大值。</summary>
    public string? Max { get; set; }

    /// <inheritdoc />
    protected override TagBuilder BuildControl()
    {
        var input = new TagBuilder("input");
        input.TagRenderMode = TagRenderMode.SelfClosing;
        input.AddCssClass("input input-bordered w-full");
        ApplySize(input);

        input.Attributes["type"] = Type;
        input.Attributes["name"] = FieldName;
        input.Attributes["id"] = FieldName;
        input.Attributes["value"] = CurrentValue;
        if (Required) input.Attributes["required"] = "required";
        if (!string.IsNullOrWhiteSpace(Placeholder)) input.Attributes["placeholder"] = Placeholder;
        if (!string.IsNullOrWhiteSpace(Step)) input.Attributes["step"] = Step;
        if (!string.IsNullOrWhiteSpace(Min)) input.Attributes["min"] = Min;
        if (!string.IsNullOrWhiteSpace(Max)) input.Attributes["max"] = Max;

        return input;
    }
}
