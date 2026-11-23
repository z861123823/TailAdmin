using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TailAdmin.Templates.Rcl.TagHelpers;

/// <summary>daisyUI 开关：&lt;ta-toggle asp-for="IsHot" label="熱銷" /&gt;。</summary>
[HtmlTargetElement("ta-toggle")]
public sealed class TaToggleTagHelper : TaFieldTagHelperBase
{
    /// <summary>开关颜色：primary / success / warning 等。</summary>
    public string Variant { get; set; } = "primary";

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(For);
        var wrapper = BuildControl();

        var error = new TagBuilder("span");
        error.AddCssClass("ta-field-error");
        error.Attributes["data-valmsg-for"] = FieldName;
        wrapper.InnerHtml.AppendHtml(error);

        output.TagName = null;
        output.Content.SetHtmlContent(wrapper);
    }

    /// <inheritdoc />
    protected override TagBuilder BuildControl()
    {
        var checkedValue = false;
        if (For.Model is bool boolValue)
        {
            checkedValue = boolValue;
        }
        else if (For.Model is not null
            && bool.TryParse(Convert.ToString(For.Model, CultureInfo.InvariantCulture), out var parsed))
        {
            checkedValue = parsed;
        }

        var wrapper = new TagBuilder("div");
        wrapper.AddCssClass("flex flex-col gap-1");

        var label = new TagBuilder("label");
        label.AddCssClass("label cursor-pointer justify-start gap-3 py-1");

        var text = new TagBuilder("span");
        text.AddCssClass("label-text text-sm font-medium");
        text.InnerHtml.Append(Label ?? For.Metadata.DisplayName ?? For.Name);
        label.InnerHtml.AppendHtml(text);

        if (Required)
        {
            var star = new TagBuilder("span");
            star.AddCssClass("ml-0.5 text-error");
            star.InnerHtml.Append("*");
            label.InnerHtml.AppendHtml(star);
        }

        var input = new TagBuilder("input");
        input.TagRenderMode = TagRenderMode.SelfClosing;
        input.AddCssClass($"toggle toggle-{Variant} toggle-sm");
        input.Attributes["type"] = "checkbox";
        input.Attributes["name"] = FieldName;
        input.Attributes["id"] = FieldName;
        input.Attributes["value"] = "true";
        if (checkedValue) input.Attributes["checked"] = "checked";
        label.InnerHtml.AppendHtml(input);

        // 未勾选时补发 false，保证 bool 模型绑定正确
        var hidden = new TagBuilder("input");
        hidden.TagRenderMode = TagRenderMode.SelfClosing;
        hidden.Attributes["type"] = "hidden";
        hidden.Attributes["name"] = FieldName;
        hidden.Attributes["value"] = "false";
        label.InnerHtml.AppendHtml(hidden);

        wrapper.InnerHtml.AppendHtml(label);

        if (!string.IsNullOrWhiteSpace(Hint))
        {
            var hint = new TagBuilder("span");
            hint.AddCssClass("ta-field-hint");
            hint.InnerHtml.Append(Hint);
            wrapper.InnerHtml.AppendHtml(hint);
        }

        return wrapper;
    }
}
