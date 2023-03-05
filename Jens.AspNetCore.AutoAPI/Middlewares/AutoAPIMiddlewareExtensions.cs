using Jens.AspNetCore.AutoAPI.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Jens.AspNetCore.AutoAPI;

public static class AutoAPIMiddlewareExtensions
{
    public static IApplicationBuilder UseAutoAPI(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionToBadRequestResponseMiddleware>();
    }
} 