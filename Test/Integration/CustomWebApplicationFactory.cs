using Microsoft.AspNetCore.Mvc.Testing;
using Server;

namespace Test;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    public Guid ContextID { get; } = Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(InMemoryDbContext));

            services.Remove(dbContextDescriptor!);
            services.AddScoped<InMemoryDbContext>((_) => new InMemoryDbContext(ContextID.ToString()));

        });

        builder.UseEnvironment("Development");
    }
}
