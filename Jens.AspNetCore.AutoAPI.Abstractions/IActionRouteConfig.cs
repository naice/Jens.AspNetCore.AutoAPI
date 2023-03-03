namespace Jens.AspNetCore.AutoAPI.Abstractions;

public record AuthorizationConfig(
    string? Policy = null,
    string? Roles = null, 
    string? AuthenticationSchemes = null);

public class ActionRouteConfig : IActionRouteConfig
{
    public string Route { get; }

    public AuthorizationConfig[]? AuthorizationConfigs { get; }

    public ActionRouteConfig(string route, AuthorizationConfig[]? authorizationConfigs)
    {
        Route = route;
        AuthorizationConfigs = authorizationConfigs;
    }
}

public interface IActionRouteConfig
{
    string Route { get; }
    AuthorizationConfig[]? AuthorizationConfigs { get; } 
}
