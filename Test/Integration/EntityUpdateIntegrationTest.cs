using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Test;

public class EntityUpdateIntegrationTest : AutoAPIIntegrationTestBase
{
    public EntityUpdateIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_EntityUpdate_Succeeds()
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
        var actor02 = await UpdateActorShouldSucceed(
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
    public async Task Ensure_EntityUpdateList_Succeeds()
    {
        await ResetAll();
        var actors01 = await CreateActorsShouldSucceed(5, "PreUpdate");
        // create controller should not update!
        var actor02 = await UpdateActorsShouldSucceed(
            actors01
                .Select(a => new Models.Actor() {
                    Id = a.Id,
                    Name = a.Id.ToString()
                })
                .ToArray());
        
        var query = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
            });

        query.Data.Should().HaveCount(5);
        query.Data.Should().AllSatisfy((actor) => actor.Name.Should().Be(actor.Id.ToString()));
    }
}
