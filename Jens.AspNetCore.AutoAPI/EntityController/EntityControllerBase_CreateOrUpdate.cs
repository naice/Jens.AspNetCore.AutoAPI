namespace Jens.AspNetCore.AutoAPI;

public abstract partial class EntityControllerBase
{
    public static async Task CreateOrUpdate<TEntity>(DbContext context, TEntity item) 
        where TEntity : class, IEntity
    {
        var dbSet = context.Set<TEntity>();
        
        if (dbSet.Where(x => x.Id == item.Id).Any())
        {
            dbSet.Update(item);
        }
        else
        {
            await dbSet.AddAsync(item);
        }
    }

    public static async Task CreateOrUpdateAndSave<TEntity>(DbContext context, TEntity item) 
        where TEntity : class, IEntity
    {
        await CreateOrUpdate<TEntity>(context, item);
        await context.SaveChangesAsync();
    }
    
    public static async Task ListCreateOrUpdateAndSave<TEntity>(DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class, IEntity
    {
        foreach (var entity in entities)
        {
            await CreateOrUpdate<TEntity>(context, entity);
        }

        await context.SaveChangesAsync();
    }

}