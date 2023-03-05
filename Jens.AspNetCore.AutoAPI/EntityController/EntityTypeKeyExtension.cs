using System.ComponentModel.DataAnnotations;
using Microsoft.OData.ModelBuilder;

namespace Jens.AspNetCore.AutoAPI;

public static class EntityTypeKeyExtension
{
    public const string EXPRESSION_PARAM_X = "x";
    
    public static void ConfigureKeys<TEntity>(this EntitySetConfiguration<TEntity> entitySetConfig)
        where TEntity : class
    {
        var entityType = typeof(TEntity);
        var keyProps = GetKeyProperties(entityType).ToArray();
        if (keyProps.Length <= 0)
            throw new InvalidOperationException($"The given entity type {entityType.FullName} has no keys defined.");		
        if (keyProps.Length == 1)
        {
            EntitySetConfigSetSingleKey(entitySetConfig, keyProps[0]);
            return;
        }

        EntitySetConfigSetMultipleKeys(entitySetConfig, keyProps);
    }

    public static void ConfigureKeys<TEntity>(this Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        var entityType = typeof(TEntity);
        var keyNames = GetKeyProperties(entityType)
            .Select(prop => prop.Name)
            .ToArray();
        if (keyNames.Length <= 0)
            throw new InvalidOperationException($"The given entity type {entityType.FullName} has no keys defined.");
        builder.HasKey(keyNames);
    }

    public static void EntitySetConfigSetMultipleKeys<TEntity>(EntitySetConfiguration<TEntity> entitySetConfig, PropertyInfo[] keyProps)
        where TEntity : class
    {
        var entityType = typeof(TEntity);
        var keyProxyType = TypeBuilderAssembly.BuildOrGetPropertyProxy(entityType, keyProps);
        MethodInfo method = typeof(EntityTypeKeyExtension)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(p => p.Name == nameof(EntityTypeKeyExtension.EntitySetConfigSetMultipleKeysGeneric) && p.IsGenericMethod);
        method = method.MakeGenericMethod(entityType, keyProxyType);
        method.Invoke(null, new object?[] { entitySetConfig, keyProps });
    }

    public static void EntitySetConfigSetMultipleKeysGeneric<TEntity, TKeyProxyType>(EntitySetConfiguration<TEntity> entitySetConfig, PropertyInfo[] keyProps)
        where TEntity : class
    {        
        entitySetConfig.EntityType.HasKey(BuildKeyProxyTypeExpression<TEntity, TKeyProxyType>(keyProps));
    }

    public static Expression<Func<TEntity, TKeyProxyType>> BuildKeyProxyTypeExpression<TEntity, TKeyProxyType>(PropertyInfo[] keyProps)
    {
        var proxyType = typeof(TKeyProxyType);
        var expressions = new List<Expression>();
        var param = Expression.Parameter(typeof(TEntity), EXPRESSION_PARAM_X);
        var expr = Expression.Lambda<Func<TEntity, TKeyProxyType>>(Expression.New(proxyType), param);
        return expr;
        // TODO: if we ever decide to need ODATA to really identify, 
        // cant get my head over it how to define the expression for this properly.
        // 
        // 
        // var variable = Expression.Variable(proxyType, "ptype");
        // var variableNew = Expression.Assign(variable, Expression.New(proxyType));
        // expressions.Add(variableNew);
        // var propertyAssignments = keyProps.Select(keyProp => 
        //     Expression.Assign(
        //         Expression.PropertyOrField(variable, keyProp.Name),
        //         Expression.PropertyOrField(param, keyProp.Name)
        //     )
        // );
        // expressions.AddRange(propertyAssignments);
        // expressions.Add(variable);
        // var body = Expression.Block(new [] { param }, expressions);
        // var expr = Expression.Lambda<Func<TEntity, TKeyProxyType>>(body, param);
        // return expr;
    }

    public static void EntitySetConfigSetSingleKey<TEntity>(EntitySetConfiguration<TEntity> entitySetConfig, PropertyInfo keyProp)
        where TEntity : class
    {
        var keyType = keyProp.PropertyType;
        MethodInfo method = typeof(EntityTypeKeyExtension)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(p => p.Name == nameof(EntityTypeKeyExtension.EntitySetConfigSingleHasKeyGeneric) && p.IsGenericMethod);
        method = method.MakeGenericMethod(typeof(TEntity), keyType);
        method.Invoke(null, new object?[] { entitySetConfig, keyProp });
    }

    public static void EntitySetConfigSingleHasKeyGeneric<TEntity, TKey>(EntitySetConfiguration<TEntity> entitySetConfig, PropertyInfo keyProp)
        where TEntity : class
    {
        entitySetConfig.EntityType.HasKey(BuildKeyAccessExpression<TEntity, TKey>(keyProp));
    }

    public static Expression<Func<TEntity, TKey>> BuildKeyAccessExpression<TEntity, TKey>(PropertyInfo keyProp)
    {
            var param = Expression.Parameter(typeof(TEntity), EXPRESSION_PARAM_X);
            var body = Expression.PropertyOrField(param, keyProp.Name);
            var expr = Expression.Lambda<Func<TEntity, TKey>>(body, param);
            return expr;
    }

    public static Expression<Func<TEntity, bool>> BuildKeyEqualityExpression<TEntity>(this TEntity entity)
    {
        var type = typeof(TEntity);
        var keyProps = GetKeyProperties(type);

        Expression? bodyExpr = null;
        var parameter = Expression.Parameter(typeof(TEntity), EXPRESSION_PARAM_X);
        foreach (var keyProp in keyProps)
        {
            var equalsExpr = Expression.Equal(
                Expression.PropertyOrField(parameter, keyProp.Name),
                Expression.Constant(keyProp.GetValue(entity)));
            if (bodyExpr == null)
            {
                bodyExpr = equalsExpr;
                continue;
            }
            
            bodyExpr = Expression.And(bodyExpr, equalsExpr);
        }

        if (bodyExpr == null)
            throw new InvalidOperationException($"The given entity type {type.FullName} has no keys defined.");

        return Expression.Lambda<Func<TEntity, bool>>(bodyExpr, parameter);
    }

    public static IEnumerable<PropertyInfo> GetKeyProperties(Type type)
    {
        return GetPropertiesByAttribute<KeyAttribute>(type);
    }

    public static IEnumerable<PropertyInfo> GetPropertiesByAttribute<TAttr>(Type type)
        => GetPropertiesByAttribute(type, typeof(TAttr));
    
    public static IEnumerable<PropertyInfo> GetPropertiesByAttribute(Type type, Type attibuteType)
    {
        foreach (var propertyInfo in type.GetProperties())
        {
            var attributeTypes = propertyInfo
                .GetCustomAttributes(false)
                .Select(attr => attr.GetType());
            if (!attributeTypes.Any(attrType => attibuteType == attrType))
                continue;
            yield return propertyInfo;
        }
    }

}