namespace Jens.AspNetCore.AutoAPI;

public interface IControllerConfigBuilder
{
    EntityControllerConfig? BuildControllerConfig(TypeInfo routeType);
}
