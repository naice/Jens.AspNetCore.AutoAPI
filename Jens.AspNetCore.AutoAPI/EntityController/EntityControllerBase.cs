namespace Jens.AspNetCore.AutoAPI;

// TODO: Refactor

public abstract class EntityControllerBase : ControllerBase
{
    public static TResponse CreateDataSuccessResponse<TResponse, TEntity>(bool success, TEntity entity)
        where TEntity : class, IEntity
        where TResponse : DataResponse<TEntity>, new()
    {
        var response = new TResponse();
        response.Data.Add(entity);
        response.Success = true;
        return response;
    }

    public static TResponse CreateDataSuccessResponse<TResponse, TEntity>(bool success, IEnumerable<TEntity> entities)
        where TEntity : class, IEntity
        where TResponse : DataResponse<TEntity>, new()
    {
        var response = new TResponse();
        response.Data.AddRange(entities);
        response.Success = true;
        return response;
    }

    public static TResponse CreateQueryResponse<TResponse, TEntity>(IQueryable<TEntity> query, QueryRequest request) 
        where TEntity : class, IEntity
        where TResponse : QueryResponse<TEntity>
    {
        query = query.ApplyQuery(request.Filter, request.Pagination, request.Sorting);

        var responseType = typeof(TResponse);
        var response = request.CloneInto(responseType) as TResponse;
        if (response == null)
            throw new InvalidOperationException($"Cloning type {responseType} from {request.GetType()} has failed.");
        
        response.Data = query.ToList();
        response.Success = true;
        
        return response;
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

    public static async Task ListCreateAndSave<TEntity>(DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class, IEntity
    {
        foreach (var entity in entities)
        {
            await Create<TEntity>(context, entity);
        }

        await context.SaveChangesAsync();
    }

    public static async Task Create<TEntity>(DbContext context, TEntity item) 
        where TEntity : class, IEntity
    {
        var dbSet = context.Set<TEntity>();
        await dbSet.AddAsync(item);
    }

    public static async Task CreateAndSave<TEntity>(DbContext context, TEntity item) 
        where TEntity : class, IEntity
    {
        await Create(context, item);
        await context.SaveChangesAsync();
    }

    public static Task Delete<TEntity>(DbContext context, TEntity item) where TEntity : class
    {
        var dbSet = context.Set<TEntity>();
        dbSet.Remove(item);
        return Task.CompletedTask;
    }
    
    public static async Task DeleteAndSave<TEntity>(DbContext context, TEntity item) where TEntity : class
    {
        await Delete(context, item);
        await context.SaveChangesAsync();
    }

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

}