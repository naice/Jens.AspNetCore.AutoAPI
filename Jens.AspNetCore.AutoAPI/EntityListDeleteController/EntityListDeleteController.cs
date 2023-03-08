namespace Jens.AspNetCore.AutoAPI;

public class EntityListDeleteController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity[]>
    where TContext : DbContext
    where TEntity : class
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    private readonly IInterceptorProvider _interceptorProvider;

    public EntityListDeleteController(TContext dbContext, IInterceptorProvider interceptorProvider)
    {
        _dbContext = dbContext;
        _interceptorProvider = interceptorProvider;
    }

    public async Task<IActionResult> ControllerAction([FromBody] TEntity[] entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities), "Controller was called with malformed entity.");
        }
        
        var itemDeleteInterceptor = new Lazy<IDeleteInterceptor<TContext, TEntity>?>(()
            => _interceptorProvider.GetInterceptor<IDeleteInterceptor<TContext, TEntity>>());
        var any = false;
        foreach (var entity in entities)
        {
            var context = new CRUDContext<TEntity, TContext>(_dbContext, entity);
            var interceptor = itemDeleteInterceptor.Value;
            var intercepted = interceptor?.BeforeDelete == null ? null : await interceptor.BeforeDelete(context);
            if (intercepted != null) return intercepted;
            if (interceptor?.Delete == null || !await interceptor.Delete(context))
            {
                var dbSet = _dbContext.Set<TEntity>();
                dbSet.Remove(context.Entity);
            }
            intercepted = interceptor?.AfterDeleted == null ? null : await interceptor.AfterDeleted(context);
            if (intercepted != null) return intercepted;
            any = true;
        }
        
        if (any)
            await _dbContext.SaveChangesAsync();
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, entities));
    }
}