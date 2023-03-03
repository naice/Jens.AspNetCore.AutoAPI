using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Models;

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithQCUD]
[WithCreateOrUpdateList]
public class Movie : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithQCUD]
[WithCreateOrUpdateList]
public class Actor : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithQCUD]
[WithCreateOrUpdateList]
public class Cast : IEntity
{
    public Guid Id { get; set; }
    public Guid ActorId { get; set; }
    public Guid MovieId { get; set; }
}

