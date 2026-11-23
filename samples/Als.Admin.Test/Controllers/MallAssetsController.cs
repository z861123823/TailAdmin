using Als.Admin.Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Als.Admin.Test.Controllers;

/// <summary>商城静态资源代理：把 /mall-assets/... 转发到商城根地址，避免图片 404。</summary>
[Route("mall-assets")]
public sealed class MallAssetsController : Controller
{
    private readonly AlsApiClient _api;

    /// <summary>构造函数。</summary>
    public MallAssetsController(AlsApiClient api) => _api = api;

    /// <summary>读取商城静态资源。</summary>
    /// <param name="path">相对商城的资源路径。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [HttpGet("{**path}")]
    public async Task<IActionResult> Get(string path, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(path) || path.Contains("..", StringComparison.Ordinal))
        {
            return NotFound();
        }

        var (bytes, contentType) = await _api.GetAssetAsync(path, cancellationToken);
        return bytes is null ? NotFound() : File(bytes, contentType);
    }
}
