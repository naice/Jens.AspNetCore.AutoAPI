using Jens.AspNetCore.AutoAPI;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Server;

public class CreateActorInterceptor : ICreateInterceptor<InMemoryDbContext, Models.Actor>
{
    public Func<CRUDContext<Actor, InMemoryDbContext>, Task<bool>>? Create { get; } 
        = (_) => Task.FromResult<bool>(false);
    Func<CRUDContext<Actor, InMemoryDbContext>, Task<IActionResult?>>? ICreateInterception<Actor, InMemoryDbContext>.BeforeCreate { get; }
        = (_) => Task.FromResult<IActionResult?>(null);
    Func<CRUDContext<Actor, InMemoryDbContext>, Task<IActionResult?>>? ICreateInterception<Actor, InMemoryDbContext>.AfterCreate { get; }
        = (_) => Task.FromResult<IActionResult?>(null);
}