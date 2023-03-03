namespace Jens.AspNetCore.AutoAPI.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class AutoAPIAuthorizationAttribute : Attribute
{
    public AutoAPIAuthorizationAttribute(
        string? Policy = null,
        string? Roles = null, 
        string? AuthenticationSchemes = null)
    {
        this.Policy = Policy;
        this.Roles = Roles;
        this.AuthenticationSchemes = AuthenticationSchemes;
    }

    public string? Policy { get; }
    public string? Roles { get; }
    public string? AuthenticationSchemes { get; }
}
