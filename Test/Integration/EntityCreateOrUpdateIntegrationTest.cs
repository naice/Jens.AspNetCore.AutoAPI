using System.Net;
using Jens.AspNetCore.AutoAPI;
using Jens.AspNetCore.AutoAPI.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Test;

public class EntityCreateOrUpdateIntegrationTest : AutoAPIIntegrationTestBase
{
    public EntityCreateOrUpdateIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_EntityCreateOrUpdate_Succeeds()
    {
        await ResetAll();
        const string actor01Name = nameof(actor01Name);
        var actor01 = await CreateOrUpdateActorShouldSucceed(
            new Models.Actor() {
                Id = Guid.Empty,
                Name = actor01Name
            });
        actor01.Name.Should().Be(actor01Name);
        const string actor02Name = nameof(actor02Name);
        var actor02 = await CreateOrUpdateActorShouldSucceed(
            new Models.Actor() {
                Id = actor01.Id,
                Name = actor02Name
            });
        actor02.Name.Should().Be(actor02Name);

        actor01.Id.Should().Be(actor02.Id);
        actor01.Name.Should().NotBe(actor02.Name);

        var query = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
            });

        query.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task Ensure_EntityCreateList_Succeeds()
    {
        await ResetAll();
        var actors = await CreateActorsShouldSucceed(5);
        // create controller should not update!
        var responseConflict = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actors,
            EntityListCreateControllerConfigBuilder.ACTION);
        responseConflict.StatusCode.Should().Be(HttpStatusCode.BadRequest, responseConflict.Model?.Message);
        responseConflict.Model!.Data.Should().BeEmpty();
        responseConflict.Model.Message.Should().NotBeNullOrEmpty();
        responseConflict.Model.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Ensure_EntityListCreateOrUpdate_Succeeds()
    {
        await ResetAll();
        var actorsInput = GenerateActors(5, "Input").ToArray();
        var actor01 = await CreateOrUpdateListActorsShouldSucceed(actorsInput);
        actor01.Should().AllSatisfy(a => a.Name.Should().StartWith("Input"));
        var actor02 = await CreateOrUpdateListActorsShouldSucceed(
            actor01
                .Select(a => 
                    new Models.Actor() {
                        Id = a.Id,
                        Name = a.Id.ToString()
                    })
                .ToArray());
        actor02.Should().AllSatisfy(a => a.Name.Should().Be(a.Id.ToString()));
        var query = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
            });

        query.Data.Should().HaveCount(5);
    }
}
