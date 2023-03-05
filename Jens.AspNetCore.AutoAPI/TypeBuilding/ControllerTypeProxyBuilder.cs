using System.Reflection.Emit;

namespace Jens.AspNetCore.AutoAPI;

public class ControllerTypeProxyBuilder
{
	public Type Type { get; }
	public Lazy<TypeBuilder> TypeBuilder { get; }
	public Type ProxyType => TypeBuilder.Value.CreateType();

    public ControllerTypeProxyBuilder(Type type) 
        : this(type, "Proxy") { }
	
	public ControllerTypeProxyBuilder(Type type, string typeSuffix)
	{
		Type = type;
		TypeBuilder = new Lazy<TypeBuilder>(() 
			=> {
				var typeBuilder = TypeBuilderAssembly.Module.Value.DefineType(
					Type.Name + typeSuffix,
					TypeAttributes.Public | TypeAttributes.Sealed,
					Type);
				typeBuilder.CreatePassThroughConstructors(Type);
				return typeBuilder;
			});
	}
	
	public ControllerTypeProxyBuilder WithTagsAttribute(params string[] tags)
	{
		var typeBuilder = TypeBuilder.Value;
		var attribType = typeof(TagsAttribute);
		var attribCtorInfo = attribType.GetConstructors().First();
		var attrBuilder = new CustomAttributeBuilder(attribCtorInfo, new object[] { tags });
		typeBuilder.SetCustomAttribute(attrBuilder);
		
		return this;
	}
    
    public ControllerTypeProxyBuilder WithDefaultProducesResponseTypeAttributes(Type responseType)
    {
        return 
            WithProducesResponseTypeAttribute(responseType, StatusCodes.Status200OK).
            WithProducesResponseTypeAttribute(typeof(BadRequestResponse), StatusCodes.Status400BadRequest);
    }
    

    public ControllerTypeProxyBuilder WithProducesResponseTypeAttribute(Type responseType, int statusCode)
    {
		var typeBuilder = TypeBuilder.Value;
		var attribType = typeof(ProducesResponseTypeAttribute);
		var attribCtorInfo = attribType.GetConstructors().Where(c => {
            var parameters = c.GetParameters();
            if (parameters.Length != 2) return false;
            return 
                parameters[0].ParameterType == typeof(Type) &&
                parameters[1].ParameterType == typeof(int);
        }).First();
		var attrBuilder = new CustomAttributeBuilder(attribCtorInfo, new object[] { responseType, statusCode });
		typeBuilder.SetCustomAttribute(attrBuilder);
        return this;
    }
}
