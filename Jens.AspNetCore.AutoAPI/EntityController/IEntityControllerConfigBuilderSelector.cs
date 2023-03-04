namespace Jens.AspNetCore.AutoAPI;

public interface IEntityControllerConfigBuilderSelector
{
    IReadOnlyList<IEntityControllerConfigBuilder> All { get; }
    IEnumerable<IEntityControllerConfigBuilder> Select(Type routeType);
}
