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

    public static IEntityControllerConfigBuilderSelector CreateDefaultSelector(IDbContextProvider contextProvider)
        => new EntityControllerBuilderByAttributeSelector()
            .With(new ByAttribute(
                    new EntityQueryControllerConfigBuilder(contextProvider, EntityQueryControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithQueryAttribute)))
            .With(new ByAttribute(
                    new EntityCreateControllerConfigBuilder(contextProvider, EntityCreateControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithCreateAttribute)))
            .With(new ByAttribute(
                    new EntityListCreateControllerConfigBuilder(contextProvider, EntityListCreateControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithCreateListAttribute)))
            .With(new ByAttribute(
                    new EntityCreateOrUpdateControllerConfigBuilder(contextProvider, EntityCreateOrUpdateControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithCreateOrUpdateAttribute)))
            .With(new ByAttribute(
                    new EntityListCreateOrUpdateControllerConfigBuilder(contextProvider, EntityListCreateOrUpdateControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithCreateOrUpdateListAttribute)))
            .With(new ByAttribute(
                    new EntityUpdateControllerConfigBuilder(contextProvider, EntityUpdateControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithUpdateAttribute)))
            .With(new ByAttribute(
                    new EntityListUpdateControllerConfigBuilder(contextProvider, EntityListUpdateControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithUpdateListAttribute)))
            .With(new ByAttribute(
                    new EntityDeleteControllerConfigBuilder(contextProvider, EntityDeleteControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithDeleteAttribute)))
            .With(new ByAttribute(
                    new EntityListDeleteControllerConfigBuilder(contextProvider, EntityListDeleteControllerConfigBuilder.ACTION),
                    typeof(WithAllAttribute),
                    typeof(WithDeleteListAttribute)));

    public static void AddAutoAPIControllers(this IServiceCollection services, IDbContextProvider contextProvider, Assembly[] scanAssemblies, Action<AutoAPIConfiguration>? configure)
    {
        var selector = CreateDefaultSelector(contextProvider);
        var config = new AutoAPIConfiguration(
            contextProvider,
            selector
        );
        configure?.Invoke(config);
        var autoAPIBuilder = new EntityControllerConfigsBuilder(scanAssemblies, selector);
        var configs = autoAPIBuilder.CreateControllerConfigs();

        services.AddMvc(options => options.Conventions.Add(new EntityControllerRouteConvention(configs)))
                .ConfigureApplicationPartManager(a => a.FeatureProviders.Add(new EntityControllerFeatureProvider(configs)));
    }
}