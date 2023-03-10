namespace Jens.AspNetCore.AutoAPI;

public abstract partial class EntityControllerBase 
    : ControllerBase 
{
    public static TResponse CreateDataSuccessResponse<TResponse, TEntity>(bool success, TEntity entity)
        where TEntity : class
        where TResponse : DataResponse<TEntity>, new()
    {
        var response = new TResponse();
        response.Data ??= new List<TEntity>();
        response.Data.Add(entity);
        response.Success = true;
        return response;
    }

    public static TResponse CreateDataSuccessResponse<TResponse, TEntity>(bool success, IEnumerable<TEntity> entities)
        where TEntity : class
        where TResponse : DataResponse<TEntity>, new()
    {
        var response = new TResponse();
        response.Data ??= new List<TEntity>();
        response.Data.AddRange(entities);
        response.Success = true;
        return response;
    }

    public static TResponse CreateQueryResponse<TResponse, TEntity>(IQueryable<TEntity> query, QueryRequest request) 
        where TEntity : class
        where TResponse : QueryResponse<TEntity>
    {
        var responseType = typeof(TResponse);
        var response = request.CloneInto(responseType) as TResponse;
        if (response == null)
            throw new InvalidOperationException($"Cloning type {responseType} from {request.GetType()} has failed.");
        
        response.Data = query.ToList();
        response.Success = true;
        
        return response;
    }
}
