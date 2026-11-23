using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Alys.TailAdmin.Rcl.TagHelpers;

/// <summary>Alys.TailAdmin 下拉框：&lt;ta-select asp-for="TypeId" items="Model.TypeList" /&gt;。</summary>
[HtmlTargetElement("ta-select")]
public sealed class TaSelectTagHelper : TaFieldTagHelperBase
{
    /// <summary>选项集合。</summary>
    public IEnumerable<SelectListItem>? Items { get; set; }

    /// <summary>是否包含「請選擇」空项，默认 true。</summary>
    public bool IncludeEmpty { get; set; } = true;

    /// <summary>空项文案。</summary>
    public string EmptyText { get; set; } = "請選擇";

    /// <inheritdoc />
    protected override TagBuilder BuildControl()
    {
        var select = new TagBuilder("select");
        select.AddCssClass(ControlClass);
        select.AddCssClass("h-11 appearance-none");
        select.Attributes["name"] = FieldName;
        select.Attributes["id"] = FieldName;
        if (Required) select.Attributes["required"] = "required";

        if (IncludeEmpty)
        {
            select.InnerHtml.AppendHtml(BuildOption(string.Empty, EmptyText, false));
        }

        var current = CurrentValue;
        if (Items is not null)
        {
            foreach (var item in Items)
            {
                var selected = string.Equals(item.Value, current, StringComparison.OrdinalIgnoreCase);
                select.InnerHtml.AppendHtml(BuildOption(item.Value ?? string.Empty, item.Text ?? string.Empty, selected));
            }
        }

        return select;
    }

    private static TagBuilder BuildOption(string value, string text, bool selected)
    {
        var option = new TagBuilder("option");
        option.Attributes["value"] = value;
        if (selected)
        {
            option.Attributes["selected"] = "selected";
        }

        option.InnerHtml.Append(text);
        return option;
    }
}
