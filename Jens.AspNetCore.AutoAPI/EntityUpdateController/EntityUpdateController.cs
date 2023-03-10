namespace Jens.AspNetCore.AutoAPI;

public class EntityUpdateController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity>
    where TContext : DbContext
    where TEntity : class
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    private readonly IInterceptorProvider _interceptorProvider;

    public EntityUpdateController(TContext dbContext, IInterceptorProvider interceptorProvider)
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
        var updateInterceptor = _interceptorProvider.GetInterceptor<IUpdateInterceptor<TContext, TEntity>>();
        var context = new CRUDContext<TEntity, TContext>(_dbContext, entity);
        bool? isUpdate = updateInterceptor?.IsUpdate == null ? null : await updateInterceptor.IsUpdate(context);
        var dbSet = _dbContext.Set<TEntity>();
        if (isUpdate == null)
        {
            isUpdate = dbSet.Where(entity.BuildKeyEqualityExpression()).Any();
        }
        
        if (isUpdate.Value)
        {
            var intercepted = updateInterceptor?.BeforeUpdate == null ? null : await updateInterceptor.BeforeUpdate(context);
            if (intercepted != null) return intercepted;
            if (updateInterceptor?.Update == null || !await updateInterceptor.Update(context))
            {
                dbSet.Update(entity);
            }
            intercepted = updateInterceptor?.AfterUpdate == null ? null : await updateInterceptor.AfterUpdate(context);
            if (intercepted != null) return intercepted;
        }
        await _dbContext.SaveChangesAsync();
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, entity));
    }
}
