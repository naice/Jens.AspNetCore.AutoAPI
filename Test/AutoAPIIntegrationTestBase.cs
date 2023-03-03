using System.Net;
using System.Text;
using System.Text.Json;
using Jens.AspNetCore.AutoAPI;
using Jens.AspNetCore.AutoAPI.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Test;

public abstract class AutoAPIIntegrationTestBase 
    : IClassFixture<WebApplicationFactory<Program>>
{
    protected WebApplicationFactory<Program> Factory { get; }

    public AutoAPIIntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
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
        return (JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }), response.StatusCode);
    }

    public async Task<Models.Actor> CreateActorShouldSucceed(Models.Actor actor)
    {
        var res = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actor, 
            EntityCreateControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Data.Should().HaveCount(1);
        var entity = res.Model.Data!.First();
        entity.Id.Should().NotBe(Guid.Empty);

        return entity!;
    }

    public async Task CreateActors(int count, string name = "Name")
    {
        for (int i = 0; i < count; i++)
        {
            await CreateActorShouldSucceed( 
                new Models.Actor() {
                    Id = Guid.Empty,
                    Name = name + i
                });
        }
    }

    public async Task<QueryResponse<TEntity>> QueryEntityShouldSucceed<TEntity>(QueryRequest request)
    {
        var res = await Post<TEntity, QueryResponse<TEntity>>(
            request,
            EntityQueryControllerConfigBuilder.ACTION);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Model.Should().NotBeNull();
        res.Model!.Message.Should().BeNull();
        res.Model.Success.Should().BeTrue();
        res.Model.Data.Should().NotBeNull();
        res.Model.Pagination.Should().NotBeNull();
        res.Model.Sorting.Should().NotBeNull();
        return res.Model;
    }
}
