using System.Reflection;

namespace Jens.AspNetCore.AutoAPI.Abstractions;

public static class AutoAPIAttributeExtensions
{
    public const string ENTITY_REPLACE_TOKEN = "[entity]";
    public const string ACTION_REPLACE_TOKEN = "[action]";

    public static string TransformRoute(this Type entityType, string route, string action)
    {
        route = route.ReplaceToken(ENTITY_REPLACE_TOKEN, entityType.Name);
        route = route.ReplaceToken(ACTION_REPLACE_TOKEN, action);
        return route;
    }

    public static string ReplaceToken(this string input, string token, string with)
        => input.Replace(token, with, true, null);

    public static AuthorizationConfig[] CreateActionRouteConfig(this Type routeType)
    {
        var routeInfo = routeType.GetTypeInfo();
        var auths = 
            routeInfo.GetCustomAttributes<AutoAPIAuthorizationAttribute>()
                .Select((x) => new AuthorizationConfig(x.Policy, x.Roles, x.AuthenticationSchemes))
                .ToArray();
        return auths;
    }

    public static string GetRoute(this Type routeType)
    {
        var routeInfo = routeType.GetTypeInfo();
        var controller = routeInfo.GetCustomAttribute<AutoAPIRouteAttribute>();
        if (controller == null) 
            throw new InvalidOperationException($"There is no {nameof(AutoAPIRouteAttribute)} defined for the given type {routeType}");
        return controller.Route;
    }

    public static string[] GetTags(this Type routeType, Type entityType)
    {
        var routeInfo = routeType.GetTypeInfo();
        var controller = routeInfo.GetCustomAttribute<AutoAPIRouteAttribute>();
        if (controller == null) 
            throw new InvalidOperationException($"There is {nameof(AutoAPIRouteAttribute)} route defined for the given type {routeType}");
        
        List<string> tags = new List<string>();
        var entityName = entityType.Name;
        tags.AddRange(controller.Tags.Select((tag) => tag.ReplaceToken(ENTITY_REPLACE_TOKEN, entityName)));
        if (tags.Count <= 0)
            tags.Add(entityName);

        return tags.ToArray();
    }
}
