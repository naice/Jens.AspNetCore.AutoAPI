namespace Jens.AspNetCore.AutoAPI;

public abstract partial class EntityControllerBase
{
    public static Task Delete<TEntity>(DbContext context, TEntity item) where TEntity : class
    {
        var dbSet = context.Set<TEntity>();
        dbSet.Remove(item);
        return Task.CompletedTask;
    }

    public static async Task ListDeleteAndSave<TEntity>(DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class, IEntity
    {
        foreach (var entity in entities)
        {
            await Delete<TEntity>(context, entity);
        }

        await context.SaveChangesAsync();
    }
    
    public static async Task DeleteAndSave<TEntity>(DbContext context, TEntity item) where TEntity : class
    {
        await Delete(context, item);
        await context.SaveChangesAsync();
    }
}