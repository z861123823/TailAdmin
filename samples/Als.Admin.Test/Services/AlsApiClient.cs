using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Als.Admin.Test.Services;

/// <summary>
/// ALS 商城 HTTP 客户端。
/// 所有后台写操作都要求防伪令牌，这里统一维持 Cookie 容器，并自动从商城后台页面抓取令牌，
/// 从而让测试后台可以像真实管理员一样调用商城既有的 /Admin/* 接口，而不需要改造商城项目。
/// </summary>
public sealed class AlsApiClient
{
    private static readonly Regex TokenRegex = new(
        "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"",
        RegexOptions.Compiled);

    private static readonly Regex TokenRegexReversed = new(
        "value=\"([^\"]+)\"[^>]*name=\"__RequestVerificationToken\"",
        RegexOptions.Compiled);

    private readonly HttpClient _http;
    private readonly ILogger<AlsApiClient> _logger;
    private readonly SemaphoreSlim _tokenGate = new(1, 1);
    private string? _token;

    /// <summary>商城服务根地址。</summary>
    public string BaseUrl { get; }

    /// <summary>构造函数。</summary>
    /// <param name="configuration">应用配置。</param>
    /// <param name="logger">日志组件。</param>
    public AlsApiClient(IConfiguration configuration, ILogger<AlsApiClient> logger)
    {
        _logger = logger;
        BaseUrl = (configuration["Als:BaseUrl"] ?? "http://localhost:5200").TrimEnd('/');
        var timeout = configuration.GetValue("Als:TimeoutSeconds", 60);

        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            AllowAutoRedirect = true
        };

        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseUrl + "/"),
            Timeout = TimeSpan.FromSeconds(timeout <= 0 ? 60 : timeout)
        };
    }

    /// <summary>发起 GET 并返回原始文本。</summary>
    public async Task<string> GetTextAsync(
        string path,
        IDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(path, query);
        using var response = await _http.GetAsync(url, cancellationToken);
        var text = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("商城 GET {Url} 返回 {Status}", url, (int)response.StatusCode);
        }

        return text;
    }

    /// <summary>发起 GET 并解析 JSON；内容非法时返回 null。</summary>
    public async Task<JsonNode?> GetJsonAsync(
        string path,
        IDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default)
    {
        var text = await GetTextAsync(path, query, cancellationToken);
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        try
        {
            return JsonNode.Parse(text);
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception, "商城响应不是合法 JSON：{Path}", path);
            return null;
        }
    }

    /// <summary>提交表单（带防伪令牌）；令牌失效时自动刷新并重试一次。</summary>
    public Task<string> PostFormAsync(
        string path,
        IEnumerable<KeyValuePair<string, string?>> fields,
        CancellationToken cancellationToken = default)
        => PostFormCoreAsync(path, fields, retry: true, cancellationToken);

    /// <summary>提交 multipart 表单（带防伪令牌），用于图片上传。</summary>
    public async Task<string> PostMultipartAsync(
        string path,
        MultipartFormDataContent content,
        CancellationToken cancellationToken = default)
    {
        var token = await GetTokenAsync(cancellationToken);
        content.Add(new StringContent(token), "__RequestVerificationToken");

        using var response = await _http.PostAsync(path, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            // 令牌过期：让下一次请求重新抓取，避免一直 400
            await RefreshTokenAsync(cancellationToken);
            _logger.LogWarning("商城 multipart 提交被拒绝（可能令牌过期）：{Path}", path);
        }

        return body;
    }

    /// <summary>读取商城静态资源（商品图、轮播图等）。</summary>
    public async Task<(byte[]? Bytes, string ContentType)> GetAssetAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        var url = path.TrimStart('/');
        using var response = await _http.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return (null, "application/octet-stream");
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        return (bytes, contentType);
    }

    /// <summary>把上传文件转成 multipart 内容。</summary>
    public static MultipartFormDataContent CreateFileContent(IFormFile file)
    {
        var content = new MultipartFormDataContent();
        var stream = new StreamContent(file.OpenReadStream());
        stream.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
        content.Add(stream, "file", file.FileName);
        return content;
    }

    private async Task<string> PostFormCoreAsync(
        string path,
        IEnumerable<KeyValuePair<string, string?>> fields,
        bool retry,
        CancellationToken cancellationToken)
    {
        var token = await GetTokenAsync(cancellationToken);
        var payload = fields
            .Where(field => field.Value is not null)
            .Select(field => new KeyValuePair<string, string>(field.Key, field.Value!))
            .ToList();
        payload.Add(new KeyValuePair<string, string>("__RequestVerificationToken", token));

        using var content = new FormUrlEncodedContent(payload);
        using var response = await _http.PostAsync(path, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest && retry)
        {
            _logger.LogWarning("商城提交被拒绝（可能令牌过期），刷新令牌后重试：{Path}", path);
            await RefreshTokenAsync(cancellationToken);
            return await PostFormCoreAsync(path, fields, retry: false, cancellationToken);
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("商城 POST {Path} 返回 {Status}", path, (int)response.StatusCode);
        }

        return body;
    }

    private async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_token))
        {
            return _token;
        }

        await _tokenGate.WaitAsync(cancellationToken);
        try
        {
            if (!string.IsNullOrEmpty(_token))
            {
                return _token;
            }

            _token = await FetchTokenAsync(cancellationToken);
            return _token;
        }
        finally
        {
            _tokenGate.Release();
        }
    }

    private async Task RefreshTokenAsync(CancellationToken cancellationToken)
    {
        await _tokenGate.WaitAsync(cancellationToken);
        try
        {
            _token = null;
            _token = await FetchTokenAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "刷新商城防伪令牌失败");
        }
        finally
        {
            _tokenGate.Release();
        }
    }

    private async Task<string> FetchTokenAsync(CancellationToken cancellationToken)
    {
        // 商城后台布局会在页面末尾渲染隐藏的防伪令牌，任一后台页都能取到
        using var response = await _http.GetAsync("Admin/Goods/Index", cancellationToken);
        var html = await response.Content.ReadAsStringAsync(cancellationToken);

        var match = TokenRegex.Match(html);
        if (!match.Success)
        {
            match = TokenRegexReversed.Match(html);
        }

        return match.Success
            ? match.Groups[1].Value
            : throw new InvalidOperationException("未能在商城后台页面中获取防伪令牌，请确认商城服务可访问。");
    }

    private static string BuildUrl(string path, IDictionary<string, string?>? query)
    {
        var url = path.StartsWith('/') ? path[1..] : path;
        if (query is null || query.Count == 0)
        {
            return url;
        }

        var pairs = query
            .Where(item => item.Value is not null)
            .Select(item => $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value!)}");
        return url + "?" + string.Join("&", pairs);
    }
}
