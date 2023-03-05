using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.Swagger;

namespace Jens.AspNetCore.AutoAPI;

public static class SwaggerExtensions
{
    public static bool TryRunSwaggerOutputOnly(this IHost app, string documentName = "v1")
    {
        if (!Environment.GetCommandLineArgs().TryGetCommand("--swaggeronly", out var path) || string.IsNullOrEmpty(path))
            return false;

        var logger = app.Services.GetRequiredService<ILogger>();
        logger.LogInformation($"Generating swagger file: {path}");
        var swaggerProvider = app.Services.GetRequiredService<ISwaggerProvider>();
        var swaggerDocument = swaggerProvider.GetSwagger(documentName);
        path = Environment.ExpandEnvironmentVariables(path);
        using var streamWriter = File.CreateText(path);
        var writer = new OpenApiYamlWriter(streamWriter);
        swaggerDocument.SerializeAsV3(writer);
        streamWriter.Close();
        logger.LogInformation($"Generating swagger file completed.");
        return true;
    }

    public static bool TryGetCommand(this string[] args, string key, out string? value)
    {
        value = null;
        for (int i = 0; i < args.Length; i++)
        {
            var akey = args[i].Trim();
            if (akey != key) continue;
            var nextIndex = i+1;
            if (nextIndex >= args.Length) 
                return false;
            var val = args[nextIndex].Trim();
            value = val.Trim('"');
            return true;
        }
        
        return false;
    }
}