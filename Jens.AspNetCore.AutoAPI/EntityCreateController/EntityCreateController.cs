namespace Jens.AspNetCore.AutoAPI;

public class EntityCreateController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity>
    where TContext : DbContext
    where TEntity : class
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    private readonly IInterceptorProvider _interceptorProvider;

    public EntityCreateController(TContext dbContext, IInterceptorProvider interceptorProvider)
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
        var interceptor = _interceptorProvider.GetInterceptor<ICreateInterceptor<TContext, TEntity>>();
        var intercepted = interceptor?.BeforeCreate == null ? null : await interceptor.BeforeCreate(context);
        if (intercepted != null) return intercepted;
        if (interceptor?.Create == null || !await interceptor.Create(context))
        {
            var dbSet = _dbContext.Set<TEntity>();
            await dbSet.AddAsync(context.Entity);
        }
        intercepted = interceptor?.AfterCreate == null ? null : await interceptor.AfterCreate(context);
        if (intercepted != null) return intercepted;
        await _dbContext.SaveChangesAsync();
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, entity));
    }
}
