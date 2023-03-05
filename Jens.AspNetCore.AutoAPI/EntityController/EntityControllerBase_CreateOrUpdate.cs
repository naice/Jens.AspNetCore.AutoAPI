namespace Jens.AspNetCore.AutoAPI;

public abstract partial class EntityControllerBase
{
    public static async Task CreateOrUpdate<TEntity>(DbContext context, TEntity item) 
        where TEntity : class
    {
        var dbSet = context.Set<TEntity>();
        if (dbSet.Where(item.BuildKeyEqualityExpression()).Any())
        {
            dbSet.Update(item);
        }
        else
        {
            await dbSet.AddAsync(item);
        }
    }

    public static async Task CreateOrUpdateAndSave<TEntity>(DbContext context, TEntity item) 
        where TEntity : class
    {
        await CreateOrUpdate<TEntity>(context, item);
        await context.SaveChangesAsync();
    }
    
    public static async Task ListCreateOrUpdateAndSave<TEntity>(DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        foreach (var entity in entities)
        {
            await CreateOrUpdate<TEntity>(context, entity);
        }

        await context.SaveChangesAsync();
    }

}