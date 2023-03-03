using Jens.AspNetCore.AutoAPI.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jens.AspNetCore.AutoAPI;

public static class SwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions WithAutoAPIDefaults(this SwaggerGenOptions options)
    {
        options.SchemaFilter<NullablePropertiesSchemaFilter>();
        options.SchemaFilter<XEnumVarNamesSchemaFilter>();
        options.SupportNonNullableReferenceTypes();
        return options;
    } 
}
