using System.Reflection.Emit;

namespace Jens.AspNetCore.AutoAPI;

// TODO: Refactor.
public static class TypeBuilderAssembly
{
	public static Lazy<AssemblyName> DynamicAssemblyName
		=> new Lazy<AssemblyName>(()
			=> new AssemblyName(nameof(TypeBuilderAssembly)));

	public static Lazy<AssemblyBuilder> DynamicAssembly
		=> new Lazy<AssemblyBuilder>(()
			=> AssemblyBuilder.DefineDynamicAssembly(DynamicAssemblyName.Value, AssemblyBuilderAccess.Run));
	
	public static Lazy<ModuleBuilder> Module
		=> new Lazy<ModuleBuilder>(()
			=> DynamicAssembly.Value.DefineDynamicModule(DynamicAssemblyName.Value.Name!));

    public static void CreatePassThroughConstructors(this TypeBuilder builder, Type baseType)
    {
        foreach (var constructor in baseType.GetConstructors()) {
            var parameters = constructor.GetParameters();
            if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false)) {
                //throw new InvalidOperationException("Variadic constructors are not supported");
                continue;
            }

            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
            var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

            var ctor = builder.DefineConstructor(MethodAttributes.Public, constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
            for (var i = 0; i < parameters.Length; ++i) {
                var parameter = parameters[i];
                var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0) {
                    parameterBuilder.SetConstant(parameter.RawDefaultValue);
                }

                foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData())) {
                    parameterBuilder.SetCustomAttribute(attribute);
                }
            }

            foreach (var attribute in BuildCustomAttributes(constructor.GetCustomAttributesData())) {
                ctor.SetCustomAttribute(attribute);
            }

            var emitter = ctor.GetILGenerator();
            emitter.Emit(OpCodes.Nop);

            // Load `this` and call base constructor with arguments
            emitter.Emit(OpCodes.Ldarg_0);
            for (var i = 1; i <= parameters.Length; ++i) {
                emitter.Emit(OpCodes.Ldarg, i);
            }
            emitter.Emit(OpCodes.Call, constructor);

            emitter.Emit(OpCodes.Ret);
        }
    }


    private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
    {
        return customAttributes.Select(attribute => {
            var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
            var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
            var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
            var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
            var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
            return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
        }).ToArray();
    }
}

public class ControllerTypeProxyBuilder
{
	public Type Type { get; }
	public Lazy<TypeBuilder> TypeBuilder { get; }
	public Type ProxyType => TypeBuilder.Value.CreateType();

    public ControllerTypeProxyBuilder(Type type) : this(type, "Proxy")
	{
		
	}
	
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
