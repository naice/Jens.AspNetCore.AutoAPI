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
/*
    [Fact]
    public async Task Ensure_EntityCreateList_Succeeds()
    {
        await ResetAllChanges();
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
    */
}
