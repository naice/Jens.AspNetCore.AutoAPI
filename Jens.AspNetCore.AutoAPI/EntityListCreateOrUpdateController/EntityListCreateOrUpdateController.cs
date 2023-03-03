namespace Jens.AspNetCore.AutoAPI;

[Tags("Generic Create or Update List Controller")]
public class EntityListCreateOrUpdateController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity[]>
    where TContext : DbContext
    where TEntity : class, IEntity
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    public EntityListCreateOrUpdateController(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> ControllerAction([FromBody] TEntity[] entities)
    {
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(entities), "Controller was called with malformed entity.");
        }
        
        await ListCreateOrUpdateAndSave(_dbContext, entities);
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, entities));
    }
}