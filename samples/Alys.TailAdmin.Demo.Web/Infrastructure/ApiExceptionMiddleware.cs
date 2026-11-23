using System.Diagnostics;
using System.Text.Json;

namespace Alys.TailAdmin.Demo.Web.Infrastructure;

/// <summary>统一异常处理：未捕获异常返回 500 JSON，并输出 X-Trace-Id。</summary>
public sealed class ApiExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = null };
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    /// <summary>初始化中间件。</summary>
    /// <param name="next">下一个中间件。</param>
    /// <param name="logger">日志。</param>
    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>处理请求。</summary>
    /// <param name="context">HTTP 上下文。</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            _logger.LogError(ex, "未處理異常 TraceId={TraceId} Path={Path}", traceId, context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Headers["X-Trace-Id"] = traceId;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsJsonAsync(
                new { Success = false, Message = "系統繁忙，請稍後重試" },
                JsonOptions);
        }
    }
}
