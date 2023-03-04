using System.Net;
using System.Text;
using System.Text.Json;
using Jens.AspNetCore.AutoAPI;
using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Test;

public abstract class AutoAPIIntegrationTestBase 
    : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncDisposable
{
    protected CustomWebApplicationFactory<Program> Factory { get; }
    
    public AutoAPIIntegrationTestBase(CustomWebApplicationFactory<Program> factory)
    {
        Factory = factory;
    }

    public async ValueTask DisposeAsync()
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Server.InMemoryDbContext>();
            context.Actors.RemoveRange(context.Actors);
            context.Movies.RemoveRange(context.Movies);
            context.Casts.RemoveRange(context.Casts);
            await context.SaveChangesAsync();
            await context.Database.EnsureDeletedAsync();
        }
    }

    public IEnumerable<Models.Actor> GenerateActors(int count, string name)
    {
       return Enumerable.Range(0, count).Select(i => 
            new Models.Actor() {
                Id = Guid.Empty,
                Name = name + i
            });
    }

    public async Task<(TResponse? Model, HttpStatusCode StatusCode)> Post<TEntity, TResponse>(object data, string action)
    {
        var entityType = typeof(TEntity);
        var route = AutoAPIAttributeExtensions.GetRoute(entityType);
        route = AutoAPIAttributeExtensions.TransformRoute(entityType, route, action);
        using var client = Factory.CreateClient();
        var content = new StringContent(
            JsonSerializer.Serialize(data),
            Encoding.UTF8,
            "application/json"
        );
        using var response = await client.PostAsync(route, content);

        var json = await response.Content.ReadAsStringAsync();
        TResponse? model = default;
        try {
            model = JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        } catch {

        }
        return (model, response.StatusCode);
    }
    
    public async Task<QueryResponse<TEntity>> QueryEntityShouldSucceed<TEntity>(QueryRequest request)
    {
        var res = await Post<TEntity, QueryResponse<TEntity>>(
            request,
            EntityQueryControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Pagination.Should().NotBeNull();
        res.Model.Sorting.Should().NotBeNull();
        return res.Model;
    }

    public async Task<Models.Actor> CreateActorShouldSucceed(Models.Actor actor)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actor, 
            EntityCreateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(1);
        var entity = res.Model.Data!.First();
        entity.Id.Should().NotBe(Guid.Empty);

        return entity!;
    }

    public async Task<IEnumerable<Models.Actor>> CreateActorsShouldSucceed(int count, string name = "Name")
    {
        var actors = GenerateActors(count, name).ToArray();
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actors, 
            EntityListCreateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(count);
        res.Model.Data.Should().AllSatisfy((entity) => entity.Id.Should().NotBe(Guid.Empty));

        return res.Model.Data!;
    }
    

    public async Task<Models.Actor> CreateOrUpdateActorShouldSucceed(Models.Actor actor)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actor, 
            EntityCreateOrUpdateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(1);
        var entity = res.Model.Data!.First();
        entity.Id.Should().NotBe(Guid.Empty);

        return entity!;
    }

    public async Task<IEnumerable<Models.Actor>> CreateOrUpdateActorsShouldSucceed(Models.Actor[] actors)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actors, 
            EntityListCreateControllerConfigBuilder.ACTION);

        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(actors.Length);
        res.Model.Data.Should().AllSatisfy((entity) => entity.Id.Should().NotBe(Guid.Empty));

        return res.Model.Data!;
    }
}
