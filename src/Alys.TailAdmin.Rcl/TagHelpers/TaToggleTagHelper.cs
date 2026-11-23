using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Alys.TailAdmin.Rcl.TagHelpers;

/// <summary>Alys.TailAdmin 开关：&lt;ta-toggle asp-for="IsHot" label="熱銷" /&gt;。</summary>
[HtmlTargetElement("ta-toggle")]
public sealed class TaToggleTagHelper : TaFieldTagHelperBase
{
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
        wrapper.AddCssClass("mb-5 flex flex-col gap-1.5");

        var label = new TagBuilder("label");
        label.AddCssClass("inline-flex cursor-pointer items-center gap-3");

        var track = new TagBuilder("span");
        track.AddCssClass(
            "relative inline-flex h-5 w-11 shrink-0 items-center rounded-full bg-gray-200 transition-colors has-[:checked]:bg-brand-500 dark:bg-gray-700");

        var input = new TagBuilder("input");
        input.TagRenderMode = TagRenderMode.SelfClosing;
        input.AddCssClass("peer sr-only");
        input.Attributes["type"] = "checkbox";
        input.Attributes["name"] = FieldName;
        input.Attributes["id"] = FieldName;
        input.Attributes["value"] = "true";
        if (checkedValue) input.Attributes["checked"] = "checked";
        track.InnerHtml.AppendHtml(input);

        var knob = new TagBuilder("span");
        knob.AddCssClass(
            "pointer-events-none inline-block size-4 translate-x-0.5 rounded-full bg-white shadow-theme-xs transition-transform peer-checked:translate-x-5.5");
        track.InnerHtml.AppendHtml(knob);
        label.InnerHtml.AppendHtml(track);

        var text = new TagBuilder("span");
        text.AddCssClass("text-theme-sm font-medium text-gray-700 dark:text-gray-400");
        text.InnerHtml.Append(Label ?? For.Metadata.DisplayName ?? For.Name);
        label.InnerHtml.AppendHtml(text);

        if (Required)
        {
            var star = new TagBuilder("span");
            star.AddCssClass("ml-0.5 text-red-500");
            star.InnerHtml.Append("*");
            label.InnerHtml.AppendHtml(star);
        }

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
