using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TailAdmin.Templates.Rcl.TagHelpers;

/// <summary>
/// 表单字段 TagHelper 基类：统一标签、说明、必填星号与字段级错误容器。
/// </summary>
public abstract class TaFieldTagHelperBase : TagHelper
{
    /// <summary>绑定模型字段。</summary>
    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = default!;

    /// <summary>标签文案。</summary>
    public string? Label { get; set; }

    /// <summary>字段说明。</summary>
    public string? Hint { get; set; }

    /// <summary>是否必填。</summary>
    public bool Required { get; set; }

    /// <summary>尺寸：sm（默认）/ md。</summary>
    public string Size { get; set; } = "sm";

    /// <summary>视图上下文。</summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    /// <summary>字段名。</summary>
    protected string FieldName => For?.Name ?? string.Empty;

    /// <summary>当前字段值。</summary>
    protected string CurrentValue => Convert.ToString(For?.Model, CultureInfo.InvariantCulture) ?? string.Empty;

    /// <summary>是否为紧凑尺寸。</summary>
    protected bool IsSmall => !string.Equals(Size, "md", StringComparison.OrdinalIgnoreCase);

    /// <summary>构建控件。</summary>
    /// <returns>控件标签。</returns>
    protected abstract TagBuilder BuildControl();

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(For);

        var wrapper = new TagBuilder("div");
        wrapper.AddCssClass("form-control w-full mb-3");

        var label = new TagBuilder("label");
        label.AddCssClass("label py-1");
        label.Attributes["for"] = FieldName;
        var labelText = new TagBuilder("span");
        labelText.AddCssClass("label-text text-sm font-medium");
        labelText.InnerHtml.Append(Label ?? For.Metadata.DisplayName ?? For.Name);
        label.InnerHtml.AppendHtml(labelText);
        if (Required)
        {
            var star = new TagBuilder("span");
            star.AddCssClass("ml-0.5 text-error");
            star.InnerHtml.Append("*");
            label.InnerHtml.AppendHtml(star);
        }

        wrapper.InnerHtml.AppendHtml(label);
        wrapper.InnerHtml.AppendHtml(BuildControl());

        if (!string.IsNullOrWhiteSpace(Hint))
        {
            var hint = new TagBuilder("span");
            hint.AddCssClass("ta-field-hint");
            hint.InnerHtml.Append(Hint);
            wrapper.InnerHtml.AppendHtml(hint);
        }

        var error = new TagBuilder("span");
        error.AddCssClass("ta-field-error");
        error.Attributes["data-valmsg-for"] = FieldName;
        wrapper.InnerHtml.AppendHtml(error);

        output.TagName = null;
        output.Content.SetHtmlContent(wrapper);
    }

    /// <summary>为控件追加尺寸类。</summary>
    /// <param name="control">控件。</param>
    /// <param name="mdClass">md 尺寸类，默认去掉 sm 后缀。</param>
    protected void ApplySize(TagBuilder control, string? mdClass = null)
    {
        if (IsSmall)
        {
            control.AddCssClass(
                control.TagName switch
                {
                    "select" => "select-sm",
                    "textarea" => "textarea-sm",
                    _ => "input-sm"
                });
        }
        else if (!string.IsNullOrWhiteSpace(mdClass))
        {
            control.AddCssClass(mdClass);
        }
    }
}
