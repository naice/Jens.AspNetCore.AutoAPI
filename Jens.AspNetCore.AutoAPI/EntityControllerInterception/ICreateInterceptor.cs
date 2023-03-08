namespace Jens.AspNetCore.AutoAPI;

public interface ICreateInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
    Func<CRUDContext<TEntity, TContext>, Task<IActionResult?>>? BeforeCreate { get; }
    Func<CRUDContext<TEntity, TContext>, Task<bool>>? Create { get; }
    Func<CRUDContext<TEntity, TContext>, Task<IActionResult?>>? AfterCreate { get; }
}

public interface ICreateInterceptor<TContext, TEntity> : ICreateInterception<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
}