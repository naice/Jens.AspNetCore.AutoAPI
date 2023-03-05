namespace Jens.AspNetCore.AutoAPI;

public abstract partial class EntityControllerBase
{
    public static void Update<TEntity>(DbContext context, TEntity item)
        where TEntity : class
    {
        var dbSet = context.Set<TEntity>();
        dbSet.Update(item);
    }

    public static async Task ListUpdateAndSave<TEntity>(DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        foreach (var entity in entities)
        {
            Update<TEntity>(context, entity);
        }

        await context.SaveChangesAsync();
    }

    public static async Task UpdateAndSave<TEntity>(DbContext context, TEntity item) 
        where TEntity : class
    {
        Update(context, item);
        await context.SaveChangesAsync();
    }
}