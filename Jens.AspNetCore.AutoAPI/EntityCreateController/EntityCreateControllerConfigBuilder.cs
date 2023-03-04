namespace Jens.AspNetCore.AutoAPI;

public class EntityCreateControllerConfigBuilder : IEntityControllerConfigBuilder
{    
    public const string ACTION = "Create";
    private readonly IDbContextProvider _dbContextProvider;
    public string ActionName { get; set; }

    public EntityCreateControllerConfigBuilder(IDbContextProvider dbContextProvider, string actionName)
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
        var dbContextType = _dbContextProvider.GetDbContext(routeType);
        
        var responseType = typeof(DataResponse<>).MakeGenericType(entityType);
        var controllerType = typeof(EntityCreateController<,,>).MakeGenericType(dbContextType, entityType, responseType);
        var proxyBuilder = new ControllerTypeProxyBuilder(controllerType);
        proxyBuilder.WithTagsAttribute(routeType.GetTags(entityType)).WithDefaultProducesResponseTypeAttributes(responseType);
        return new EntityControllerConfig(
            proxyBuilder.ProxyType,
            route,
            authConfigs
        );
    }
}
