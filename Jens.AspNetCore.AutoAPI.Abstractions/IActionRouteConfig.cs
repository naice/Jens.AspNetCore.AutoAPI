namespace Jens.AspNetCore.AutoAPI.Abstractions;

public record AuthorizationConfig(
    string? Policy = null,
    string? Roles = null, 
    string? AuthenticationSchemes = null);

public interface IActionRouteConfig
{
    string Route { get; }
    AuthorizationConfig[]? AuthorizationConfigs { get; } 
}
