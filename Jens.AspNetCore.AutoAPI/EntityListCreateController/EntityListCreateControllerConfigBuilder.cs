namespace Jens.AspNetCore.AutoAPI;

public class EntityListCreateControllerConfigBuilder : IControllerConfigBuilder
{
    public const string ACTION = "CreateList";
    private readonly IDbContextProvider _dbContextProvider;

    public EntityListCreateControllerConfigBuilder(IDbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }
    public EntityControllerConfig? BuildControllerConfig(TypeInfo routeType)
    {
        if (routeType.GetCustomAttribute<WithCreateListAttribute>() == null) return null;
        var entityType = routeType;
        var authConfigs = routeType.CreateActionRouteConfig();
        var route = entityType.TransformRoute(
            routeType.GetRoute(),
            ACTION
        );
        var dbContextType = _dbContextProvider.GetDbContext(routeType);        
        var responseType = typeof(DataResponse<>).MakeGenericType(entityType);
        var controllerType = typeof(EntityListCreateController<,,>).MakeGenericType(dbContextType, entityType, responseType);
        var proxyBuilder = new ControllerTypeProxyBuilder(controllerType);
        proxyBuilder.WithTagsAttribute(routeType.GetTags(entityType)).WithDefaultProducesResponseTypeAttributes(responseType);
        return new EntityControllerConfig(
            proxyBuilder.ProxyType,
            route,
            authConfigs
        );
    }
}
