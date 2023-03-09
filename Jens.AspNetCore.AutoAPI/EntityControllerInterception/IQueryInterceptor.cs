namespace Jens.AspNetCore.AutoAPI;

public class QueryContext<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
    public TContext DbContext { get; }
    public Type EntityType { get; }
    public QueryRequest QueryRequest { get; }

    public QueryContext(TContext dbContext, Type entityType, QueryRequest queryRequest)
    {
        DbContext = dbContext;
        EntityType = entityType;
        QueryRequest = queryRequest;
    }
}

public interface IQueryInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
    Func<QueryContext<TEntity, TContext>, Task<IActionResult?>>? BeforeQuery { get; }
    Func<QueryContext<TEntity, TContext>, Task<IQueryable<TEntity>>>? Query { get; }
    Func<QueryContext<TEntity, TContext>, Task<IActionResult?>>? AfterQuery { get; }
}

public interface IQueryInterceptor<TContext, TEntity> : IQueryInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
}
