using System.ComponentModel.DataAnnotations;
using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Models;

/*

    Demo of how REST APIs are defined. 

*/

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithAllAttribute]
public class Movie
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithAllAttribute]
public class Actor
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[AutoAPIAuthorization(Roles: "test", AuthenticationSchemes: "Test")]
[WithAllAttribute]
public class Cast
{
    [Key]
    public Guid ActorId { get; set; }
    [Key]
    public Guid MovieId { get; set; }

    public string Value { get; set; } = string.Empty;
}

[AutoAPIRoute("/AutoAPI/[Entity][Action]")]
[WithAllAttribute]
public class Sample
{
    [Key]
    public Guid Id { get; set; }
    public Guid? NullableGuid { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public DateTime DateTime { get; set; }
}
