namespace Jens.AspNetCore.AutoAPI;

public class EntityListCreateController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity[]>
    where TContext : DbContext
    where TEntity : class, IEntity
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    public EntityListCreateController(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> ControllerAction([FromBody] TEntity[] entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities), "Controller was called with malformed entities.");
        }
        
        await ListCreateAndSave(_dbContext, entities);
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, entities));
    }
}