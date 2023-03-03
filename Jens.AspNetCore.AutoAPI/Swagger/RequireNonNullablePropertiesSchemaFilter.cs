using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Jens.AspNetCore.AutoAPI.Swagger;

public class NullablePropertiesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!schema.Properties.Any()) return;        
        var typeInfo = context.Type.GetTypeInfo();

        var additionalRequiredProps = schema.Properties
            .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key));
        var nullContext = new NullabilityInfoContext();
        foreach (var prop in additionalRequiredProps)
        {
            var propertyName = prop.Key.ToLowerInvariant();
            var propSchema = prop.Value;
            var propertyInfo = typeInfo.GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() == propertyName);
            if (propertyInfo == null) throw new Exception($"Could not find property type for propertyKey={prop.Key}");

            var nullInfo = nullContext.Create(propertyInfo);
            if (nullInfo.ReadState == NullabilityState.Nullable && nullInfo.WriteState == NullabilityState.Nullable)
                continue;
            
            schema.Required.Add(prop.Key);
        }
        foreach (var prop in schema.Properties)
        {
            var propertyName = prop.Key.ToLowerInvariant();
            var propSchema = prop.Value;
            var type = typeInfo.GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() == propertyName)?.PropertyType;
            if (type == null) throw new Exception($"Could not find property type for propertyKey={prop.Key}");

            if (type == typeof(string)) {
                propSchema.Nullable = false;
            } else if (type == typeof(DateTime?)) {
                propSchema.Nullable = false;
            } else if (type == typeof(DateTime)) {
                propSchema.Nullable = false;
            }
        }
    }
}
