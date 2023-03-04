namespace Jens.AspNetCore.AutoAPI;

public class EntityListDeleteController<TContext, TEntity, TResponse> : EntityControllerBase, IEntityControllerAction<TEntity[]>
    where TContext : DbContext
    where TEntity : class, IEntity
    where TResponse : DataResponse<TEntity>, new()
{
    private readonly TContext _dbContext;
    public EntityListDeleteController(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> ControllerAction([FromBody] TEntity[] entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Controller was called with malformed entity.");
        }
        
        await ListDeleteAndSave(_dbContext, entity);
        return Ok(CreateDataSuccessResponse<TResponse, TEntity>(true, entity));
    }
}