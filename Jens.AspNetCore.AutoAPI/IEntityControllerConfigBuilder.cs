namespace Jens.AspNetCore.AutoAPI;

public interface IEntityControllerConfigBuilder
{
    EntityControllerConfig? BuildControllerConfig(TypeInfo routeType);
}
