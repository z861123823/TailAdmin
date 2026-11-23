using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TailAdmin.Templates.Rcl.TagHelpers;

/// <summary>
/// 表单字段 TagHelper 基类：统一标签、说明、必填星号与字段级错误容器。
/// 样式以 TailAdmin 官方表单元素为准。
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

    /// <summary>视图上下文。</summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    /// <summary>字段名。</summary>
    protected string FieldName => For?.Name ?? string.Empty;

    /// <summary>当前字段值。</summary>
    protected string CurrentValue => Convert.ToString(For?.Model, CultureInfo.InvariantCulture) ?? string.Empty;

    /// <summary>官方输入控件公共类名。</summary>
    protected const string ControlClass =
        "w-full rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-theme-sm text-gray-800 shadow-theme-xs transition-colors placeholder:text-gray-400 focus:border-brand-300 focus:ring-3 focus:ring-brand-500/20 focus:outline-hidden dark:border-gray-700 dark:bg-gray-900 dark:text-white/90 dark:placeholder:text-white/30 dark:focus:border-brand-800";

    /// <summary>构建控件。</summary>
    /// <returns>控件标签。</returns>
    protected abstract TagBuilder BuildControl();

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(For);

        var wrapper = new TagBuilder("div");
        wrapper.AddCssClass("mb-5 w-full");

        var label = new TagBuilder("label");
        label.AddCssClass("mb-1.5 block text-theme-sm font-medium text-gray-700 dark:text-gray-400");
        label.Attributes["for"] = FieldName;
        label.InnerHtml.Append(Label ?? For.Metadata.DisplayName ?? For.Name);
        if (Required)
        {
            var star = new TagBuilder("span");
            star.AddCssClass("ml-0.5 text-red-500");
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
}
