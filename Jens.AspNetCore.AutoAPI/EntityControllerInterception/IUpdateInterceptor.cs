namespace Jens.AspNetCore.AutoAPI;
public interface IUpdateInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
    Func<CRUDContext<TEntity, TContext>, Task<bool>> IsUpdate { get; }
    Func<CRUDContext<TEntity, TContext>, Task<IActionResult?>>? BeforeUpdate { get; }
    Func<CRUDContext<TEntity, TContext>, Task<bool>>? Update { get; }
    Func<CRUDContext<TEntity, TContext>, Task<IActionResult?>>? AfterUpdate { get; }
}

public interface IUpdateInterceptor<TContext, TEntity> : IUpdateInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
}
