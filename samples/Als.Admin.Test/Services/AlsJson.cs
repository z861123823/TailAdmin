using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Als.Admin.Test.Services;

/// <summary>商城 JSON 读取与图片地址归一化工具。</summary>
public static class AlsJson
{
    private static readonly JsonSerializerOptions OutputOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>按大小写不敏感方式读取属性。</summary>
    public static JsonNode? Prop(JsonNode? node, string name)
    {
        if (node is not JsonObject obj)
        {
            return null;
        }

        if (obj.TryGetPropertyValue(name, out var exact))
        {
            return exact;
        }

        foreach (var pair in obj)
        {
            if (string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                return pair.Value;
            }
        }

        return null;
    }

    /// <summary>读取字符串属性。</summary>
    public static string? Str(JsonNode? node, string name)
    {
        var value = Prop(node, name);
        if (value is null)
        {
            return null;
        }

        if (value is JsonValue jsonValue && jsonValue.TryGetValue<string>(out var text))
        {
            return text;
        }

        var raw = value.ToJsonString();
        return raw.Length >= 2 && raw[0] == '"' ? raw[1..^1] : raw;
    }

    /// <summary>读取整数属性。</summary>
    public static int Int(JsonNode? node, string name, int fallback = 0)
        => int.TryParse(Str(node, name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;

    /// <summary>读取金额属性。</summary>
    public static decimal Decimal(JsonNode? node, string name, decimal fallback = 0m)
        => decimal.TryParse(Str(node, name), NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
            ? value
            : fallback;

    /// <summary>读取布尔属性，兼容 true / 1 / "true" / "1"。</summary>
    public static bool Bool(JsonNode? node, string name)
    {
        var text = Str(node, name);
        return string.Equals(text, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(text, "1", StringComparison.Ordinal)
            || string.Equals(text, "yes", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>读取数组属性。</summary>
    public static JsonArray? Array(JsonNode? node, string name) => Prop(node, name) as JsonArray;

    /// <summary>读取对象属性。</summary>
    public static JsonObject? Object(JsonNode? node, string name) => Prop(node, name) as JsonObject;

    /// <summary>
    /// 把商城返回的图片相对地址改写成测试后台的静态资源代理地址，
    /// 这样浏览器不会把 /images/xxx 解析到测试后台自己的域名下。
    /// </summary>
    public static JsonNode? PrefixAssetUrls(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var pair in obj.ToList())
                {
                    if (pair.Value is JsonValue value
                        && value.TryGetValue<string>(out var text)
                        && ShouldPrefix(pair.Key, text)
                        && text.StartsWith('/')
                        && !text.StartsWith("//", StringComparison.Ordinal)
                        && !text.StartsWith("/mall-assets/", StringComparison.OrdinalIgnoreCase))
                    {
                        obj[pair.Key] = "/mall-assets" + text;
                        continue;
                    }

                    PrefixAssetUrls(pair.Value);
                }

                break;

            case JsonArray array:
                foreach (var item in array)
                {
                    PrefixAssetUrls(item);
                }

                break;
        }

        return node;
    }

    /// <summary>把节点序列化为紧凑 JSON 文本。</summary>
    public static string ToJson(JsonNode? node) => node?.ToJsonString(OutputOptions) ?? "{}";

    /// <summary>
    /// 判断字段是否应按图片地址改写。
    /// <c>PicUrls</c> 固定为图片；<c>PicUrl</c> 在轮播图中表示跳转链接，只有图片扩展名时才改写。
    /// </summary>
    private static bool ShouldPrefix(string key, string value)
        => string.Equals(key, "PicUrls", StringComparison.OrdinalIgnoreCase)
            || (string.Equals(key, "PicUrl", StringComparison.OrdinalIgnoreCase) && IsImagePath(value));

    private static bool IsImagePath(string value)
    {
        if (value.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var path = value.Split('?', '#')[0];
        return path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>构造商城图片的浏览器可访问地址。</summary>
    public static string? AssetUrl(string? path)
        => string.IsNullOrWhiteSpace(path)
            ? path
            : path.StartsWith('/') && !path.StartsWith("//", StringComparison.Ordinal)
                ? "/mall-assets" + path
                : path;
}
