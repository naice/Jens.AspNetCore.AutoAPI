namespace Jens.AspNetCore.AutoAPI;

public class EntityDeleteController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity>
    where TContext : DbContext
    where TEntity : class
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    private readonly IInterceptorProvider _interceptorProvider;

    public EntityDeleteController(TContext dbContext, IInterceptorProvider interceptorProvider)
    {
        _dbContext = dbContext;
        _interceptorProvider = interceptorProvider;
    }

    public async Task<IActionResult> ControllerAction([FromBody] TEntity entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Controller was called with malformed entity.");
        }
        
        var context = new CRUDContext<TEntity, TContext>(_dbContext, entity);
        var interceptor = _interceptorProvider.GetInterceptor<IDeleteInterceptor<TContext, TEntity>>();
        var intercepted = interceptor?.BeforeDelete == null ? null : await interceptor.BeforeDelete(context);
        if (intercepted != null) return intercepted;
        if (interceptor?.Delete == null || !await interceptor.Delete(context))
        {
            var dbSet = _dbContext.Set<TEntity>();
            dbSet.Remove(context.Entity);
        }
        intercepted = interceptor?.AfterDeleted == null ? null : await interceptor.AfterDeleted(context);
        if (intercepted != null) return intercepted;
        await _dbContext.SaveChangesAsync();
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, context.Entity));
    }
}