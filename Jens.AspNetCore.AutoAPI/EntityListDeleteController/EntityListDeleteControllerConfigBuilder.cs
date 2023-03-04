namespace Jens.AspNetCore.AutoAPI;

public class EntityListDeleteControllerConfigBuilder : IControllerConfigBuilder
{
    public const string ACTION = "DeleteList";
    private readonly IDbContextProvider _dbContextProvider;

    public EntityListDeleteControllerConfigBuilder(IDbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }
    public EntityControllerConfig? BuildControllerConfig(TypeInfo routeType)
    {
        if (routeType.GetCustomAttribute<WithDeleteListAttribute>() == null)
            return null;
        var entityType = routeType;
        var authConfigs = routeType.CreateActionRouteConfig();
        var route = entityType.TransformRoute(
            routeType.GetRoute(),
            ACTION
        );
        var dbContextType = _dbContextProvider.GetDbContext(routeType);        
        var responseType = typeof(DataResponse<>).MakeGenericType(entityType);
        var controllerType = typeof(EntityListDeleteController<,,>).MakeGenericType(dbContextType, entityType, responseType);
        var proxyBuilder = new ControllerTypeProxyBuilder(controllerType);
        proxyBuilder.WithTagsAttribute(routeType.GetTags(entityType)).WithDefaultProducesResponseTypeAttributes(responseType);
        return new EntityControllerConfig(
            proxyBuilder.ProxyType,
            route,
            authConfigs
        );
    }
}
