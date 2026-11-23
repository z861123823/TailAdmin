using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TailAdmin.Templates.Rcl.TagHelpers;

/// <summary>TailAdmin 文本域：&lt;ta-textarea asp-for="Remark" rows="3" /&gt;。</summary>
[HtmlTargetElement("ta-textarea")]
public sealed class TaTextareaTagHelper : TaFieldTagHelperBase
{
    /// <summary>行数。</summary>
    public int Rows { get; set; } = 3;

    /// <summary>占位文本。</summary>
    public string? Placeholder { get; set; }

    /// <inheritdoc />
    protected override TagBuilder BuildControl()
    {
        var textarea = new TagBuilder("textarea");
        textarea.AddCssClass(ControlClass);
        textarea.AddCssClass("min-h-[120px]");
        textarea.Attributes["name"] = FieldName;
        textarea.Attributes["id"] = FieldName;
        textarea.Attributes["rows"] = Rows.ToString();
        if (Required) textarea.Attributes["required"] = "required";
        if (!string.IsNullOrWhiteSpace(Placeholder)) textarea.Attributes["placeholder"] = Placeholder;
        textarea.InnerHtml.Append(CurrentValue);
        return textarea;
    }
}
