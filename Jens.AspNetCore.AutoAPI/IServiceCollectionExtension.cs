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
            // Query
            new EntityQueryControllerConfigBuilder(contextProvider),
            // Create
            new EntityCreateControllerConfigBuilder(contextProvider),
            new EntityListCreateControllerConfigBuilder(contextProvider),
            // CreateOrUpdate
            new EntityCreateOrUpdateControllerConfigBuilder(contextProvider),
            new EntityListCreateOrUpdateControllerConfigBuilder(contextProvider),
            // Update
            new EntityUpdateControllerConfigBuilder(contextProvider),
            new EntityListUpdateControllerConfigBuilder(contextProvider),
            // Delte
            new EntityDeleteControllerConfigBuilder(contextProvider),
            new EntityListDeleteControllerConfigBuilder(contextProvider)
        );
        configure?.Invoke(config);
        var autoAPIBuilder = new EntityControllerConfigsBuilder(scanAssemblies, config.ControllerConfigBuilders.ToArray());
        var configs = autoAPIBuilder.CreateControllerConfigs();

        services.AddMvc(options => options.Conventions.Add(new EntityControllerRouteConvention(configs)))
                .ConfigureApplicationPartManager(a => a.FeatureProviders.Add(new EntityControllerFeatureProvider(configs)));
    }
}