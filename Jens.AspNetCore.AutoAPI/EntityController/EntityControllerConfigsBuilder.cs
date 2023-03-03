namespace Jens.AspNetCore.AutoAPI;

public class EntityControllerConfigsBuilder
{
    private readonly Assembly[] _scanAssemblies;
    private readonly IControllerConfigBuilder[] _configBuilders;

    public EntityControllerConfigsBuilder(Assembly[] scanAssemblies, IControllerConfigBuilder[] configBuilders)
    {
        _scanAssemblies = scanAssemblies;
        _configBuilders = configBuilders;
    }

    public IEnumerable<TypeInfo> GetEntityTypeRoutes()
    {
        return _scanAssemblies
            .SelectMany(asm => asm.GetTypes())
            .Select(type => type.GetTypeInfo())
            .Where(tifo => 
                tifo.IsAssignableTo(typeof(IEntity)) && 
                tifo.GetCustomAttribute<AutoAPIRouteAttribute>() != null);
    }

    public IEnumerable<EntityControllerConfig> BuildConfigs(TypeInfo routeType)
    {
        return _configBuilders
            .Select(builder => builder.BuildControllerConfig(routeType))
            .Where(x => x != null)
            .ToArray()!;
    }

    public EntityControllerConfigs CreateControllerConfigs()
    {
        var result = new EntityControllerConfigs();

        foreach (var routeType in GetEntityTypeRoutes())
        {
            result.AddRange(BuildConfigs(routeType));
        }
        return result;
    }
}
