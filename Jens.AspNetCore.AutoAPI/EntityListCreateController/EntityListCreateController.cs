namespace Jens.AspNetCore.AutoAPI;

public class EntityListCreateController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity[]>
    where TContext : DbContext
    where TEntity : class
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    private readonly IInterceptorProvider _interceptorProvider;

    public EntityListCreateController(TContext dbContext, IInterceptorProvider interceptorProvider)
    {
        _dbContext = dbContext;
        _interceptorProvider = interceptorProvider;
    }

    public async Task<IActionResult> ControllerAction([FromBody] TEntity[] entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities), "Controller was called with malformed entities.");
        }
        
        var dbSet = _dbContext.Set<TEntity>();
        var itemCreateInterceptor = new Lazy<ICreateInterceptor<TContext, TEntity>?>(()
            => _interceptorProvider.GetInterceptor<ICreateInterceptor<TContext, TEntity>>());
        var any = false;
        foreach (var entity in entities)
        {
            var context = new CRUDContext<TEntity, TContext>(_dbContext, entity);
            var interceptor = _interceptorProvider.GetInterceptor<ICreateInterceptor<TContext, TEntity>>();
            var intercepted = interceptor?.BeforeCreate == null ? null : await interceptor.BeforeCreate(context);
            if (intercepted != null) return intercepted;
            if (interceptor?.Create == null || !await interceptor.Create(context))
            {
                await dbSet.AddAsync(context.Entity);
            }
            intercepted = interceptor?.AfterCreate == null ? null : await interceptor.AfterCreate(context);
            if (intercepted != null) return intercepted;
            any = true;
        }
        
        if (any)
            await _dbContext.SaveChangesAsync();
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, entities));
    }
}