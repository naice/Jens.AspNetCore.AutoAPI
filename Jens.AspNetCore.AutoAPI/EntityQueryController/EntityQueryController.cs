namespace Jens.AspNetCore.AutoAPI;

public class EntityQueryController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<QueryRequest>
    where TContext : DbContext
    where TEntity : class
    where TResponse : QueryResponse<TEntity>
{
    private readonly TContext _dbContext;
    public EntityQueryController(TContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<IActionResult> ControllerAction([FromBody] QueryRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Controller was called with malformed entity.");

        var data = _dbContext.Set<TEntity>().AsQueryable();
        return Task.FromResult<IActionResult>(
            Ok(CreateQueryResponse<TResponse, TEntity>(data, request))
        );
    }
}
