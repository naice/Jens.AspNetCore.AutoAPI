namespace Jens.AspNetCore.AutoAPI;

public class AutoAPIConfiguration
{
    public IDbContextProvider DbContextProvider { get; set; }
    public List<IControllerConfigBuilder> ControllerConfigBuilders { get; } = new List<IControllerConfigBuilder>();

    public AutoAPIConfiguration(IDbContextProvider dbContextProvider, params IControllerConfigBuilder[] builders)
    {
        DbContextProvider = dbContextProvider;
        ControllerConfigBuilders.AddRange(builders);
    }
}
