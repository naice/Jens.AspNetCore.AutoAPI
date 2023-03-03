namespace Jens.AspNetCore.AutoAPI.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AutoAPIRouteAttribute : Attribute
{    
    public AutoAPIRouteAttribute(string Route, params string[] Tags)
    {
        this.Route = Route;
        this.Tags = Tags;
    }

    public string Route { get; }
    public IReadOnlyCollection<string> Tags { get; }
}