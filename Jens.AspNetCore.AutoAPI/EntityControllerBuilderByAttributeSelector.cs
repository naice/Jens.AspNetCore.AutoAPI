

namespace Jens.AspNetCore.AutoAPI;

public record ByAttribute(IEntityControllerConfigBuilder Builder, params Type[] AttributeTypes);

public class EntityControllerBuilderByAttributeSelector : IEntityControllerConfigBuilderSelector
{
    private readonly List<ByAttribute> _byAttributes = new List<ByAttribute>();

    public IReadOnlyList<ByAttribute> ByAttributes => _byAttributes;

    public IReadOnlyList<IEntityControllerConfigBuilder> All => _byAttributes.Select((byAttr) => byAttr.Builder).ToList();

    public EntityControllerBuilderByAttributeSelector With(ByAttribute byAttribute)
    {
        _byAttributes.Add(byAttribute);
        return this;
    }

    public IEnumerable<IEntityControllerConfigBuilder> Select(Type routeType)
    {
        var attributeTypes = routeType.GetCustomAttributes(false)
            .Select(attr => attr.GetType());

        return _byAttributes
            .Where((byAttr) => 
                byAttr.AttributeTypes.Any((attr) => attributeTypes.Contains(attr)))
            .Select((byAttr) => 
                byAttr.Builder);
    }
}
