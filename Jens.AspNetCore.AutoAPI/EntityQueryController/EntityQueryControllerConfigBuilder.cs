namespace Jens.AspNetCore.AutoAPI;

public class EntityQueryControllerConfigBuilder : IEntityControllerConfigBuilder
{
    public const string ACTION = "Query";
    private readonly IDbContextProvider _dbContextProvider;
    public string ActionName { get; set; }

    public EntityQueryControllerConfigBuilder(IDbContextProvider dbContextProvider, string actionName)
    {
        _dbContextProvider = dbContextProvider;
        ActionName = actionName;
    }
    public EntityControllerConfig? BuildControllerConfig(TypeInfo routeType)
    {
        var entityType = routeType;
        var authConfigs = routeType.CreateActionRouteConfig();
        var route = entityType.TransformRoute(
            routeType.GetRoute(),
            ActionName
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
