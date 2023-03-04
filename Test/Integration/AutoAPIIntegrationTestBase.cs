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

    public async Task ResetAll()
    {
        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Server.InMemoryDbContext>();
            context.Actors.RemoveRange(context.Actors);
            context.Movies.RemoveRange(context.Movies);
            context.Casts.RemoveRange(context.Casts);
            await context.SaveChangesAsync();
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
    public async Task<(TResponse? Model, HttpStatusCode StatusCode)> PostMalformed<TEntity, TResponse>(string action)
    {
        var entityType = typeof(TEntity);
        var route = AutoAPIAttributeExtensions.GetRoute(entityType);
        route = AutoAPIAttributeExtensions.TransformRoute(entityType, route, action);
        using var client = Factory.CreateClient();
        var content = new StringContent(
            "{ THIS SHOULD BE ALWAYS MALFORMED: JSON }, hopefully.}",
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
    
    public async Task QueryActorMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, QueryResponse<Models.Actor>>(EntityQueryControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
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

    public async Task CreateActorMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityCreateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
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

    public async Task CreateActorsMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityListCreateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
    }

    public async Task<IEnumerable<Models.Actor>> CreateActorsShouldSucceed(Models.Actor[] actors)
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
    public Task<IEnumerable<Models.Actor>> CreateActorsShouldSucceed(int count, string name = "Name")
    {
        var actors = GenerateActors(count, name).ToArray();
        return CreateActorsShouldSucceed(actors);
    }
    
    public async Task CreateOrUpdateActorMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityCreateOrUpdateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
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
    
    public async Task CreateOrUpdateListActorsMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityListCreateOrUpdateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
    }    

    public async Task<Models.Actor[]> CreateOrUpdateListActorsShouldSucceed(Models.Actor[] actors)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actors, 
            EntityListCreateOrUpdateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(actors.Length);
        res.Model.Data.Should().AllSatisfy((entity) => entity.Id.Should().NotBe(Guid.Empty));

        return res.Model.Data!.ToArray();
    }
    
    public async Task UpdateActorMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityUpdateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
    }    

    public async Task<Models.Actor> UpdateActorShouldSucceed(Models.Actor actor)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actor, 
            EntityUpdateControllerConfigBuilder.ACTION);
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
    
    public async Task UpdateActorsMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityListUpdateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
    }

    public async Task<Models.Actor> UpdateActorsShouldSucceed(Models.Actor[] actor)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actor, 
            EntityListUpdateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(actor.Length);
        var entity = res.Model.Data!.First();
        entity.Id.Should().NotBe(Guid.Empty);

        return entity!;
    }
    
    public async Task DeleteActorMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityDeleteControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
    }

    public async Task<Models.Actor> DeleteActorShouldSucceed(Models.Actor actor)
    {
        var actorId = actor.Id;
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actor,
            EntityDeleteControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(1);
        var entity = res.Model.Data!.First();
        entity.Id.Should().Be(actorId);
        return entity!;
    }
    
    public async Task DeleteActorsMalformedShouldFail()
    {
        var res = await PostMalformed<Models.Actor, DataResponse<Models.Actor>>(EntityListDeleteControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().NotBeNull();
        res.Model.Success.Should().BeFalse();
    }

    public async Task<IEnumerable<Models.Actor>> DeleteActorsShouldSucceed(Models.Actor[] actors)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actors, 
            EntityListDeleteControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK, res.Model?.Message);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(actors.Length);
        res.Model.Data.Should().AllSatisfy((entity) => actors.Single(a => entity.Id == a.Id));

        return res.Model.Data!;
    }

}
