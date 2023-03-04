namespace Jens.AspNetCore.AutoAPI;

public interface IEntityControllerAction<TEntity>
{
    Task<IActionResult> ControllerAction([FromBody] TEntity entity);
}
