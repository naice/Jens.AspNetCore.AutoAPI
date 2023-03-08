namespace Jens.AspNetCore.AutoAPI;

public class CRUDContext<TEntity, TContext>
    where TContext : DbContext
    where TEntity : class
{
    public TContext DbContext { get; }
    public TEntity Entity { get; }

    public CRUDContext(TContext dbContext, TEntity entity)
    {
        DbContext = dbContext;
        Entity = entity;
    }
}
