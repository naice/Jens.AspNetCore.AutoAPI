namespace Jens.AspNetCore.AutoAPI;

public class AutoAPIConfiguration
{
    public IDbContextProvider DbContextProvider { get; set; }
    public IEntityControllerConfigBuilderSelector EntityControllerConfigBuilderSelector { get; set; }

    public AutoAPIConfiguration(IDbContextProvider dbContextProvider, IEntityControllerConfigBuilderSelector entityControllerConfigBuilderSelector)
    {
        DbContextProvider = dbContextProvider;
        EntityControllerConfigBuilderSelector = entityControllerConfigBuilderSelector;
    }
}
