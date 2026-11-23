using System.Text.Json;

namespace Als.Admin.Test.Infrastructure;

/// <summary>把未处理异常统一转成前端可读的 JSON，避免页面直接吞掉异常。</summary>
public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    /// <summary>构造函数。</summary>
    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>执行中间件。</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // 浏览器主动取消，无需记录
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "请求 {Path} 处理失败", context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";
            var message = JsonSerializer.Serialize(new
            {
                Success = "false",
                Message = "測試後台處理請求失敗：" + exception.Message
            });
            await context.Response.WriteAsync(message);
        }
    }
}
