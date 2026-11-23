using Microsoft.AspNetCore.Html;

namespace Alys.TailAdmin.Rcl.Models;

/// <summary>警告提示模型，对应 TailAdmin 的 Alert 组件。</summary>
public sealed class TaAlertModel
{
    /// <summary>提示样式：default / info / success / warning / danger，默认 info。</summary>
    public string Variant { get; set; } = "info";

    /// <summary>标题，留空时只显示内容。</summary>
    public string? Title { get; set; }

    /// <summary>提示内容（纯文本，HTML 会被编码）。</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>提示内的链接地址。</summary>
    public string? LinkUrl { get; set; }

    /// <summary>提示内的链接文字，与 LinkUrl 同时设置才显示。</summary>
    public string? LinkText { get; set; }

    /// <summary>是否显示状态图标。</summary>
    public bool ShowIcon { get; set; } = true;

    /// <summary>是否显示关闭按钮。</summary>
    public bool Dismissible { get; set; }

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>头像模型。</summary>
public sealed class TaAvatarModel
{
    /// <summary>头像图片地址，留空时使用首字母占位。</summary>
    public string? Src { get; set; }

    /// <summary>图片替代文本。</summary>
    public string? Alt { get; set; }

    /// <summary>无图片时显示的首字母（建议 1-2 个字符）。</summary>
    public string? Initials { get; set; }

    /// <summary>尺寸：xs / sm / md / lg / xl / 2xl，默认 md。</summary>
    public string Size { get; set; } = "md";

    /// <summary>形状：circle（默认）/ rounded。</summary>
    public string Shape { get; set; } = "circle";

    /// <summary>在线状态：none（默认）/ online / offline / busy。</summary>
    public string Status { get; set; } = "none";

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>头像组模型，用于重叠展示多个头像。</summary>
public sealed class TaAvatarGroupModel
{
    /// <summary>头像集合。</summary>
    public List<TaAvatarModel> Items { get; set; } = new();

    /// <summary>最多展示的头像数量，超出部分折叠为 +N，默认 4。</summary>
    public int Max { get; set; } = 4;

    /// <summary>总数，大于 Max 时显示 +N；为空时取 Items.Count。</summary>
    public int? Total { get; set; }

    /// <summary>尺寸：xs / sm / md / lg / xl / 2xl，默认 md。</summary>
    public string Size { get; set; } = "md";
}

/// <summary>徽章模型。</summary>
public sealed class TaBadgeModel
{
    /// <summary>徽章文字。</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>颜色：primary / success / warning / danger / info / light / dark，默认 primary。</summary>
    public string Variant { get; set; } = "primary";

    /// <summary>尺寸：sm / md，默认 md。</summary>
    public string Size { get; set; } = "md";

    /// <summary>是否显示前置圆点。</summary>
    public bool Dot { get; set; }

    /// <summary>前置图标名（对应 _Icon）。</summary>
    public string? Icon { get; set; }

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>面包屑模型。</summary>
public sealed class TaBreadcrumbModel
{
    /// <summary>面包屑节点（最后一项自动作为当前页）。</summary>
    public List<TaBreadcrumbItemModel> Items { get; set; } = new();

    /// <summary>是否在首位显示首页，默认 true。</summary>
    public bool ShowHome { get; set; } = true;

    /// <summary>首页链接地址。</summary>
    public string HomeUrl { get; set; } = "/";

    /// <summary>首页文字。</summary>
    public string HomeText { get; set; } = "首頁";

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>面包屑节点。</summary>
public sealed class TaBreadcrumbItemModel
{
    /// <summary>节点文字。</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>节点链接，留空表示不可点击。</summary>
    public string? Url { get; set; }
}

/// <summary>按钮模型。</summary>
public sealed class TaButtonModel
{
    /// <summary>按钮文字。</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>样式：primary / success / warning / danger / outline / ghost / light，默认 primary。</summary>
    public string Variant { get; set; } = "primary";

    /// <summary>尺寸：sm / md / lg，默认 md。</summary>
    public string Size { get; set; } = "md";

    /// <summary>图标名（对应 _Icon）。</summary>
    public string? Icon { get; set; }

    /// <summary>图标位置：left（默认）/ right。</summary>
    public string IconPosition { get; set; } = "left";

    /// <summary>链接地址；设置后渲染为 a 标签。</summary>
    public string? Href { get; set; }

    /// <summary>a 标签的 target。</summary>
    public string? Target { get; set; }

    /// <summary>按钮类型：button（默认）/ submit / reset。</summary>
    public string Type { get; set; } = "button";

    /// <summary>点击时执行的内联脚本（可选）。</summary>
    public string? OnClick { get; set; }

    /// <summary>是否禁用。</summary>
    public bool Disabled { get; set; }

    /// <summary>是否占满可用宽度。</summary>
    public bool FullWidth { get; set; }

    /// <summary>无文字时的无障碍标签。</summary>
    public string? AriaLabel { get; set; }

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>按钮组模型，用于分段式按钮。</summary>
public sealed class TaButtonGroupModel
{
    /// <summary>按钮集合，每个按钮可单独指定 Variant 表示选中态。</summary>
    public List<TaButtonModel> Items { get; set; } = new();

    /// <summary>尺寸：sm / md / lg，默认 md。</summary>
    public string Size { get; set; } = "md";

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>卡片模型。复杂内容可通过 Body 传入受信任的 HTML 片段。</summary>
public sealed class TaCardModel
{
    /// <summary>标题。</summary>
    public string? Title { get; set; }

    /// <summary>副标题。</summary>
    public string? Subtitle { get; set; }

    /// <summary>正文纯文本（与 Body 二选一）。</summary>
    public string? Text { get; set; }

    /// <summary>正文 HTML（受信任内容，不会被再次编码）。</summary>
    public IHtmlContent? Body { get; set; }

    /// <summary>头部图标名（对应 _Icon）。</summary>
    public string? Icon { get; set; }

    /// <summary>右上角链接地址。</summary>
    public string? Href { get; set; }

    /// <summary>右上角链接文字，默认「查看全部」。</summary>
    public string LinkText { get; set; } = "查看全部";

    /// <summary>底部说明文字。</summary>
    public string? FooterText { get; set; }

    /// <summary>是否启用内边距，默认 true。</summary>
    public bool Padding { get; set; } = true;

    /// <summary>是否启用悬停阴影。</summary>
    public bool Hoverable { get; set; }

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>轮播模型。</summary>
public sealed class TaCarouselModel
{
    /// <summary>轮播项。</summary>
    public List<TaCarouselItemModel> Items { get; set; } = new();

    /// <summary>高度：sm（h-48）/ md（h-64）/ lg（h-80），默认 md。</summary>
    public string Height { get; set; } = "md";

    /// <summary>是否显示左右切换按钮。</summary>
    public bool ShowArrows { get; set; } = true;

    /// <summary>是否显示底部指示点。</summary>
    public bool ShowIndicators { get; set; } = true;

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>轮播项。</summary>
public sealed class TaCarouselItemModel
{
    /// <summary>图片地址。</summary>
    public string Src { get; set; } = string.Empty;

    /// <summary>图片替代文本。</summary>
    public string? Alt { get; set; }

    /// <summary>标题浮层。</summary>
    public string? Caption { get; set; }

    /// <summary>描述浮层。</summary>
    public string? Description { get; set; }

    /// <summary>点击跳转地址。</summary>
    public string? Href { get; set; }
}

/// <summary>下拉菜单模型。</summary>
public sealed class TaDropdownModel
{
    /// <summary>触发按钮文字。</summary>
    public string TriggerText { get; set; } = "更多";

    /// <summary>触发按钮图标（对应 _Icon），默认不显示图标。</summary>
    public string? TriggerIcon { get; set; }

    /// <summary>触发按钮样式：outline（默认）/ primary / ghost / light。</summary>
    public string Variant { get; set; } = "outline";

    /// <summary>触发按钮尺寸：sm / md / lg，默认 md。</summary>
    public string Size { get; set; } = "md";

    /// <summary>菜单对齐：left（默认）/ right。</summary>
    public string Align { get; set; } = "left";

    /// <summary>菜单宽度：sm（w-40）/ md（w-48）/ lg（w-60），默认 md。</summary>
    public string Width { get; set; } = "md";

    /// <summary>菜单项。</summary>
    public List<TaDropdownItemModel> Items { get; set; } = new();

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>下拉菜单项。</summary>
public sealed class TaDropdownItemModel
{
    /// <summary>菜单文字。</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>图标名（对应 _Icon）。</summary>
    public string? Icon { get; set; }

    /// <summary>链接地址；与 OnClick 二选一。</summary>
    public string? Url { get; set; }

    /// <summary>点击脚本（点击后自动关闭菜单）。</summary>
    public string? OnClick { get; set; }

    /// <summary>是否危险操作（红色）。</summary>
    public bool Danger { get; set; }

    /// <summary>是否在此项前加分隔线。</summary>
    public bool DividerBefore { get; set; }

    /// <summary>是否禁用。</summary>
    public bool Disabled { get; set; }
}

/// <summary>下拉选择框模型，用于 _TaSelect 局部视图（Alpine 自定义列表，可深度美化）。</summary>
public sealed class TaSelectModel
{
    /// <summary>表单字段名，作为隐藏输入框的 name。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>元素 id，留空时取 Name。</summary>
    public string? Id { get; set; }

    /// <summary>当前选中值。</summary>
    public string? Value { get; set; }

    /// <summary>未选择时的占位文案，默认「請選擇」。</summary>
    public string Placeholder { get; set; } = "請選擇";

    /// <summary>选项集合。</summary>
    public List<TaSelectOptionModel> Options { get; set; } = new();

    /// <summary>是否禁用。</summary>
    public bool Disabled { get; set; }

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>下拉选择框选项。</summary>
public sealed class TaSelectOptionModel
{
    /// <summary>选项值。</summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>选项文字。</summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>图片画廊模型。</summary>
public sealed class TaImagesModel
{
    /// <summary>图片集合。</summary>
    public List<TaImageItemModel> Items { get; set; } = new();

    /// <summary>列数：2 / 3（默认）/ 4。</summary>
    public int Columns { get; set; } = 3;

    /// <summary>比例：square（默认）/ video / portrait / wide / auto。</summary>
    public string Ratio { get; set; } = "square";

    /// <summary>是否圆角。</summary>
    public bool Rounded { get; set; } = true;

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>图片项。</summary>
public sealed class TaImageItemModel
{
    /// <summary>图片地址。</summary>
    public string Src { get; set; } = string.Empty;

    /// <summary>图片替代文本。</summary>
    public string? Alt { get; set; }

    /// <summary>图片下方说明文字。</summary>
    public string? Caption { get; set; }

    /// <summary>点击跳转地址。</summary>
    public string? Href { get; set; }
}

/// <summary>链接模型。</summary>
public sealed class TaLinkModel
{
    /// <summary>链接文字。</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>链接地址。</summary>
    public string Href { get; set; } = "#";

    /// <summary>样式：primary（默认）/ muted / underline。</summary>
    public string Variant { get; set; } = "primary";

    /// <summary>前置图标名（对应 _Icon）。</summary>
    public string? Icon { get; set; }

    /// <summary>是否在新窗口打开（自动追加外链图标）。</summary>
    public bool External { get; set; }

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>列表模型。</summary>
public sealed class TaListModel
{
    /// <summary>列表项。</summary>
    public List<TaListItemModel> Items { get; set; } = new();

    /// <summary>是否显示分隔线，默认 true。</summary>
    public bool Divided { get; set; } = true;

    /// <summary>附加类名。</summary>
    public string? CssClass { get; set; }
}

/// <summary>列表项。</summary>
public sealed class TaListItemModel
{
    /// <summary>主文字。</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>说明文字。</summary>
    public string? Description { get; set; }

    /// <summary>图标名（对应 _Icon）。</summary>
    public string? Icon { get; set; }

    /// <summary>链接地址；为空时渲染为普通条目。</summary>
    public string? Href { get; set; }

    /// <summary>右侧徽章文字。</summary>
    public string? Badge { get; set; }

    /// <summary>是否高亮当前项。</summary>
    public bool Active { get; set; }
}

/// <summary>组件样式映射，供 Partial 复用，避免类名散落。</summary>
public static class TaUiStyle
{
    /// <summary>按钮基础类。</summary>
    public const string ButtonBase = "inline-flex items-center justify-center rounded-lg font-medium shadow-theme-xs transition-colors disabled:cursor-not-allowed disabled:opacity-60";

    /// <summary>获取按钮样式类。</summary>
    /// <param name="variant">样式变体。</param>
    /// <param name="size">尺寸。</param>
    /// <returns>Tailwind 类名。</returns>
    public static string Button(string variant, string size)
    {
        var color = (variant ?? string.Empty).ToLowerInvariant() switch
        {
            "success" => "bg-green-500 text-white hover:bg-green-600",
            "warning" => "bg-amber-500 text-white hover:bg-amber-600",
            "danger" => "bg-red-500 text-white hover:bg-red-600",
            "outline" => "border border-gray-300 bg-white text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-white/5",
            "ghost" => "text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/5",
            "light" => "bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-white/5 dark:text-gray-300 dark:hover:bg-white/10",
            _ => "bg-brand-500 text-white hover:bg-brand-600"
        };

        var sizing = (size ?? string.Empty).ToLowerInvariant() switch
        {
            "sm" => "gap-1.5 px-3 py-2 text-theme-xs [&_svg]:size-4",
            "lg" => "gap-2 px-5 py-3.5 text-theme-sm [&_svg]:size-5",
            _ => "gap-2 px-4 py-2.5 text-theme-sm [&_svg]:size-4"
        };

        return $"{ButtonBase} {color} {sizing}";
    }
}
