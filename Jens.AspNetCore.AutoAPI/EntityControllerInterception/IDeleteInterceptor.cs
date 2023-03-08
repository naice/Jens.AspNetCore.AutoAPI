namespace Jens.AspNetCore.AutoAPI;

public interface IDeleteInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
    Func<CRUDContext<TEntity, TContext>, Task<IActionResult?>>? BeforeDelete { get; }
    Func<CRUDContext<TEntity, TContext>, Task<bool>>? Delete { get; }
    Func<CRUDContext<TEntity, TContext>, Task<IActionResult?>>? AfterDeleted { get; }
}
public interface IDeleteInterceptor<TContext, TEntity> : IDeleteInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
}
