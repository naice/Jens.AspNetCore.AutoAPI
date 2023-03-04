using System.Net;
using Jens.AspNetCore.AutoAPI;
using Jens.AspNetCore.AutoAPI.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Test;

public class EntityCreateIntegrationTest : AutoAPIIntegrationTestBase
{
    public EntityCreateIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_EntityCreate_Succeeds()
    {
        var entity = await CreateActorShouldSucceed( 
            new Models.Actor() {
                Id = Guid.Empty,
                Name = "Name"
            });
        
        // create controller should not update!
        var responseConflict = await Post<Models.Actor, DataResponse<Models.Actor>>(
            new Models.Actor() {
                Id = entity.Id,
                Name = "Nameasdf"
            }, 
            EntityCreateControllerConfigBuilder.ACTION);
        responseConflict.StatusCode.Should().Be(HttpStatusCode.BadRequest, responseConflict.Model?.Message);
        responseConflict.Model!.Data.Should().BeEmpty();
        responseConflict.Model.Message.Should().NotBeNullOrEmpty();
        responseConflict.Model.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Ensure_EntityCreateList_Succeeds()
    {
        var actors = await CreateActorsShouldSucceed(5);
        // create controller should not update!
        var responseConflict = await Post<Models.Actor, DataResponse<Models.Actor>>(
            actors,
            EntityListCreateControllerConfigBuilder.ACTION);
        responseConflict.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseConflict.Model!.Data.Should().BeEmpty();
        responseConflict.Model.Message.Should().NotBeNullOrEmpty();
        responseConflict.Model.Success.Should().BeFalse();
    }
}
