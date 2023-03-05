using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;

namespace Jens.AspNetCore.AutoAPI;

public static class IQueryableExtension
{
    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, Sorting sorting)
    {
        var type = typeof(T);
        var propertyInfo = type.GetProperties().FirstOrDefault(nfo => nfo.Name.ToLowerInvariant() == sorting.Field.ToLowerInvariant());

        var param = Expression.Parameter(typeof(T), "x");
        var body = Expression.Convert(ExpandPropertyAccess(sorting.Field, param), typeof(object));
        var expr = Expression.Lambda<Func<T, object>>(body, param);

        if (sorting.Direction == SortingDirection.DESCENDING)
        {
            return query.OrderByDescending(expr);
        }
        return query.OrderBy(expr);
    }

    public static MemberExpression ExpandPropertyAccess(string propAccess, ParameterExpression param)
    {
        var split = propAccess.Split('/', StringSplitOptions.TrimEntries);
        if (split.Length <= 1)
            return Expression.PropertyOrField(param, propAccess);

        var expr = Expression.PropertyOrField(param, split[0]);
        for (int i = 1; i < split.Length; i++)
        {
            expr = Expression.PropertyOrField(expr, split[i]);
        }
        
        return expr;
    }

    public static IQueryable<T> WithPagination<T>(this IQueryable<T> query, Pagination pagination) 
    {
        return query.Skip((int)(pagination.Page * pagination.PageSize)).Take((int)pagination.PageSize);
    } 

    public static IQueryable<T> ApplyODataFilter<T>(this IQueryable<T> data, string filterString) where T : class
    {
        ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
        builder.EnableLowerCamelCase();
        var entityType = typeof(T).GetTypeInfo();
        var entitySetConfig = builder.EntitySet<T>(entityType.Name);
        entitySetConfig.ConfigureKeys();

        ODataQueryContext context = new ODataQueryContext(builder.GetEdmModel(), typeof(T), new ODataPath());
        ODataQueryOptionParser queryOptionParser = new ODataQueryOptionParser(
            context.Model,
            context.ElementType,
            context.NavigationSource,
            new Dictionary<string, string> { { "$filter", filterString } });
        FilterQueryOption filter = new FilterQueryOption(filterString, context, queryOptionParser);
        IQueryable query = filter.ApplyTo(data, new ODataQuerySettings());
        return query.Cast<T>();
    }

    public static IQueryable<TData> ApplyQuery<TData>(this IQueryable<TData> query, string? filter, Pagination pagination, Sorting sorting)
        where TData : class
    {
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.ApplyODataFilter<TData>(filter);
        }
        pagination.Total = query.Count();
        query = query.SortBy(sorting);
        query = query.WithPagination(pagination);

        return query;
    }
}
