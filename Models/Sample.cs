using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Models;

/*

    Demo of how REST APIs are defined. 

*/

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithAllAttribute]
public class Movie : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithAllAttribute]
public class Actor : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithAllAttribute]
public class Cast : IEntity
{
    public Guid Id { get; set; }
    public Guid ActorId { get; set; }
    public Guid MovieId { get; set; }
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithAllAttribute]
public class Sample : IEntity
{
    public Guid Id { get; set; }
    public Guid? NullableGuid { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public DateTime DateTime { get; set; }
}

