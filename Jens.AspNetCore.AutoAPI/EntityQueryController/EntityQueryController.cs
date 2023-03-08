namespace Jens.AspNetCore.AutoAPI;

public class EntityQueryController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<QueryRequest>
    where TContext : DbContext
    where TEntity : class
    where TResponse : QueryResponse<TEntity>
{
    private readonly TContext _dbContext;
    private readonly IInterceptorProvider _interceptorProvider;

    public EntityQueryController(TContext dbContext, IInterceptorProvider interceptorProvider)
    {
        _dbContext = dbContext;
        _interceptorProvider = interceptorProvider;
    }
    
    public async Task<IActionResult> ControllerAction([FromBody] QueryRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Controller was called with malformed entity.");

        var query = _dbContext.Set<TEntity>().AsQueryable();
        var context = new QueryContext<TEntity, TContext>(_dbContext, typeof(TEntity), query, request);

        var interceptor = _interceptorProvider.GetInterceptor<IQueryInterceptor<TContext, TEntity>>();
        var intercepted = interceptor?.BeforeQuery == null ? null : await interceptor.BeforeQuery(context);
        if (intercepted != null) return intercepted;
        var interceptedQuery = interceptor?.Query == null ? null : await interceptor.Query(context);
        query = interceptedQuery ?? query.ApplyQuery(request.Filter, request.Pagination, request.Sorting);
        intercepted = interceptor?.AfterQuery == null ? null : await interceptor.AfterQuery(context);
        if (intercepted != null) return intercepted;

        return Ok(CreateQueryResponse<TResponse, TEntity>(query, request));
    }
}
