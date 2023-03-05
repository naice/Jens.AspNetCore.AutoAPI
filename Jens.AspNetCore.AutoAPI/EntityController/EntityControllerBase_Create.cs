namespace Jens.AspNetCore.AutoAPI;

public abstract partial class EntityControllerBase
{
    public static async Task Create<TEntity>(DbContext context, TEntity item) 
        where TEntity : class
    {
        var dbSet = context.Set<TEntity>();
        await dbSet.AddAsync(item);
    }

    public static async Task ListCreateAndSave<TEntity>(DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        foreach (var entity in entities)
        {
            await Create<TEntity>(context, entity);
        }

        await context.SaveChangesAsync();
    }

    public static async Task CreateAndSave<TEntity>(DbContext context, TEntity item) 
        where TEntity : class
    {
        await Create(context, item);
        await context.SaveChangesAsync();
    }
}