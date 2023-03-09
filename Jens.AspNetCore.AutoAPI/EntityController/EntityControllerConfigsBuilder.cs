namespace Jens.AspNetCore.AutoAPI;

public class EntityControllerConfigsBuilder
{
    private readonly Assembly[] _scanAssemblies;
    private readonly IEntityControllerConfigBuilderSelector _entityControllerConfigBuilderSelector;

    private readonly Lazy<IEnumerable<TypeInfo>> _typeInfos;

    public EntityControllerConfigsBuilder(Assembly[] scanAssemblies, IEntityControllerConfigBuilderSelector entityControllerConfigBuilderSelector)
    {
        _scanAssemblies = scanAssemblies;
        _entityControllerConfigBuilderSelector = entityControllerConfigBuilderSelector;
        _typeInfos = new Lazy<IEnumerable<TypeInfo>>(() 
            => _scanAssemblies
                .SelectMany(asm => asm.GetTypes())
                .Select(type => type.GetTypeInfo())
                .ToArray());
    }

    public IEnumerable<TypeInfo> GetEntityTypeRoutes()
    {
        return _typeInfos.Value
            .Where(tifo => tifo.GetCustomAttribute<AutoAPIRouteAttribute>() != null);
    }

    public IEnumerable<EntityControllerConfig> BuildConfigs(TypeInfo routeType)
    {
        return _entityControllerConfigBuilderSelector
            .Select(routeType)
            .Select(builder => builder.BuildControllerConfig(routeType))
            .Where(x => x != null)
            .ToArray()!;
    }

    public IEnumerable<(Type ServiceType, Type ImplementationType)> GetInterceptors()
    {
        foreach (var type in _typeInfos.Value)
        {
            var interfaces = type.GetInterfaces();
            var candidates = interfaces
                .Where(ifc => 
                {
                    if (!ifc.IsGenericType) 
                        return false;
                    var gtd = ifc.GetGenericTypeDefinition();
                    if (gtd == typeof(ICreateInterceptor<,>) || 
                        gtd == typeof(IUpdateInterceptor<,>) || 
                        gtd == typeof(IDeleteInterceptor<,>) || 
                        gtd == typeof(IQueryInterceptor<,>))
                        return true;
                    return false;
                });
            
            if (!candidates.Any())
                continue;

            foreach (var candidate in candidates)
            {
                yield return (candidate, type);
            }
        }
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
