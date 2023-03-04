namespace Jens.AspNetCore.AutoAPI;

public class EntityControllerConfigsBuilder
{
    private readonly Assembly[] _scanAssemblies;
    private readonly IEntityControllerConfigBuilderSelector _entityControllerConfigBuilderSelector;

    public EntityControllerConfigsBuilder(Assembly[] scanAssemblies, IEntityControllerConfigBuilderSelector entityControllerConfigBuilderSelector)
    {
        _scanAssemblies = scanAssemblies;
        _entityControllerConfigBuilderSelector = entityControllerConfigBuilderSelector;
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
        return _entityControllerConfigBuilderSelector
            .Select(routeType)
            .Select(builder => builder.BuildControllerConfig(routeType))
            .Where(x => x != null)
            .ToArray()!;
    }

    public EntityControllerConfigs CreateControllerConfigs()
    {
        var result = new EntityControllerConfigs();

        foreach (var routeType in GetEntityTypeRoutes())
        {
            var configs = BuildConfigs(routeType);
            if (!configs.Any())
            {
                // TODO: Maybe log this?
                // route type detected but yielded no configuration.
            }
            result.AddRange(BuildConfigs(routeType));
        }
        return result;
    }
}
