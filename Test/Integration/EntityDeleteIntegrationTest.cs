using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Test;

public class EntityDeleteIntegrationTest : AutoAPIIntegrationTestBase
{
    public EntityDeleteIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_EntityDelete_Succeeds()
    {
        await ResetAll();
        var actor01 = await CreateActorShouldSucceed( 
            new Models.Actor() {
                Id = Guid.Empty,
                Name = "Name"
            });
        var actorGuid = actor01.Id;

        await DeleteActorShouldSucceed(actor01);

        var query01 = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
            });

        query01.Data.Should().HaveCount(0);
        
        var actor02 = await CreateActorShouldSucceed( 
            new Models.Actor() {
                Id = actorGuid,
                Name = "Name"
            });

        var query02 = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
            });
        query02.Data.Should().HaveCount(1);
        query02.Data.Should().AllSatisfy((x) => x.Id.Should().Be(actorGuid));
    }

    [Fact]
    public async Task Ensure_EntityDeleteList_Succeeds()
    {
        await ResetAll();
        var actors01 = (await CreateActorsShouldSucceed(5)).ToArray();
        var actors01Ids = actors01.Select(a => a.Id).ToArray();

        await DeleteActorsShouldSucceed(actors01);

        var query01 = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
            });

        query01.Data.Should().HaveCount(0);
        
        var actor02 = await CreateActorsShouldSucceed(
            actors01Ids.Select(id => 
                new Models.Actor() {
                    Id = id,
                    Name = "Name"
                })
                .ToArray()
            );
        var query02 = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
            });
        query02.Data.Should().HaveCount(5);
        query02.Data.Should().AllSatisfy((actor) =>  actors01Ids.Single(id => actor.Id == id));
    }
}
