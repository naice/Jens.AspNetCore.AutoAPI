using Microsoft.AspNetCore.Mvc;
using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Jens.AspNetCore.AutoAPI;

public interface IEntityControllerAction<TEntity>
{
    Task<IActionResult> ControllerAction([FromBody] TEntity entity);
}
