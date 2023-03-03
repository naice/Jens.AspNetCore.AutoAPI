namespace Jens.AspNetCore.AutoAPI;

public record EntityControllerConfig(
    Type ControllerType,
    string Route,
    AuthorizationConfig[]? AuthorizationConfigs = null
) : IActionRouteConfig;
