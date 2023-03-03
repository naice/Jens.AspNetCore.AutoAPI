using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jens.AspNetCore.AutoAPI.Swagger;

public class XEnumVarNamesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            var array = new OpenApiArray();
            array.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
            // NSwag
            schema.Extensions.Add("x-enumNames", array);
            // Openapi-generator
            schema.Extensions.Add("x-enum-varnames", array);
        }
    }
}
