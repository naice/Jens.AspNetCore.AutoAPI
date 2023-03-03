namespace Jens.AspNetCore.AutoAPI;

public class EntityQueryControllerConfigBuilder : IControllerConfigBuilder
{
    public const string ACTION = "Query";
    private readonly IDbContextProvider _dbContextProvider;

    public EntityQueryControllerConfigBuilder(IDbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }
    public EntityControllerConfig? BuildControllerConfig(TypeInfo routeType)
    {
        if (routeType.GetCustomAttribute<WithQCUDAttribute>() == null &&
            routeType.GetCustomAttribute<WithQueryAttribute>() == null) return null;
        var entityType = routeType;
        var authConfigs = routeType.CreateActionRouteConfig();
        var route = entityType.TransformRoute(
            routeType.GetRoute(),
            ACTION
        );
        var dbContextType = _dbContextProvider.GetDbContext(entityType);
        
        var responseType = typeof(QueryResponse<>).MakeGenericType(entityType);
        var controllerType = typeof(EntityQueryController<,,>).MakeGenericType(dbContextType, entityType, responseType).GetTypeInfo();
        var proxyBuilder = new ControllerTypeProxyBuilder(controllerType);
        proxyBuilder.WithTagsAttribute(routeType.GetTags(entityType)).WithDefaultProducesResponseTypeAttributes(responseType);
        return new EntityControllerConfig(
            proxyBuilder.ProxyType,
            route,
            authConfigs
        );
    }
}
