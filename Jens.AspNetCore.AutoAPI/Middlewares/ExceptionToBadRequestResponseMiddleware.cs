using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Jens.AspNetCore.AutoAPI.Middlewares;

public class ExceptionToBadRequestResponseMiddleware
{
    private readonly RequestDelegate _nextRequest;
    private readonly ILogger<ExceptionToBadRequestResponseMiddleware> _logger;

    public ExceptionToBadRequestResponseMiddleware(RequestDelegate nextRequest, ILogger<ExceptionToBadRequestResponseMiddleware> logger)
    {
        _nextRequest = nextRequest;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _nextRequest.Invoke(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request thrown unhandled.");
            
            await HandleExceptionMessageAsync(context, ex)
                .ConfigureAwait(false);
        }
    }

    protected static Task HandleExceptionMessageAsync(HttpContext context, Exception ex)
    {
        string response = JsonSerializer.Serialize(new BadRequestResponse()
        {
            Success = false,
            Message = ResolveMessages(ex),
        });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsync(response);
    }

    protected static string ResolveMessages(Exception? exception)
    {
        if (exception == null) return "";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(exception.Message);
        sb.AppendLine(ResolveMessages(exception.InnerException));
        return sb.ToString();
    }
}
