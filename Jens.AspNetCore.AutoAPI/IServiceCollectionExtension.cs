using Microsoft.Extensions.DependencyInjection;

namespace Jens.AspNetCore.AutoAPI;

public static class IServiceCollectionExtension 
{
    public static void AddAutoAPIControllers<TDbContext>(
            this IServiceCollection services,
            params Assembly[] assemblies)
        where TDbContext : DbContext
        => AddAutoAPIControllers(
            services,
            new EntityDbContextTypeConfiguration<TDbContext>(),
            assemblies,
            null);

    public static void AddAutoAPIControllers<TDbContext>(
            this IServiceCollection services,
            Action<AutoAPIConfiguration> configure,
            params Assembly[] assemblies)
        where TDbContext : DbContext
        => AddAutoAPIControllers(
            services,
            new EntityDbContextTypeConfiguration<TDbContext>(),
            assemblies,
            configure);

    public static void AddAutoAPIControllers(this IServiceCollection services, IDbContextProvider contextProvider, Assembly[] scanAssemblies, Action<AutoAPIConfiguration>? configure)
    {
        var config = new AutoAPIConfiguration(
            contextProvider,
            new EntityQueryControllerConfigBuilder(contextProvider),
            new EntityCreateControllerConfigBuilder(contextProvider),
            new EntityCreateOrUpdateControllerConfigBuilder(contextProvider),
            new EntityDeleteControllerConfigBuilder(contextProvider),
            new EntityListCreateControllerConfigBuilder(contextProvider),
            new EntityListCreateOrUpdateControllerConfigBuilder(contextProvider)
        );
        configure?.Invoke(config);
        var autoAPIBuilder = new EntityControllerConfigsBuilder(scanAssemblies, config.ControllerConfigBuilders.ToArray());
        var configs = autoAPIBuilder.CreateControllerConfigs();

        services.AddMvc(options => options.Conventions.Add(new EntityControllerRouteConvention(configs)))
                .ConfigureApplicationPartManager(a => a.FeatureProviders.Add(new EntityControllerFeatureProvider(configs)));
    }
}