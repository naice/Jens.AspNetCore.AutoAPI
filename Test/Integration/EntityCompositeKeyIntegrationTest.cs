using Jens.AspNetCore.AutoAPI.Abstractions;

namespace Test;

public class EntityCompositeKeyIntegrationTest : AutoAPIIntegrationTestBase
{
    public EntityCompositeKeyIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_Entities_With_Composite_Keys_Succeed()
    {
        await ResetAll();
        var actorId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var createInput01 = new Models.Cast() { ActorId = actorId, MovieId = Guid.NewGuid(), Value = "Value01" };
        var createInput02 = new Models.Cast() { ActorId = actorId, MovieId = movieId, Value = "Value02" };
        var createInput03 = new Models.Cast() { ActorId = actorId, MovieId = Guid.NewGuid(), Value = "Value03" };
        var createInput04 = new Models.Cast() { ActorId = Guid.NewGuid(), MovieId = movieId, Value = "Value04" };
        var createInput05 = new Models.Cast() { ActorId = Guid.NewGuid(), MovieId = Guid.NewGuid(), Value = "Value05" };
        var createInput = new [] {
            createInput01, createInput02, createInput03, createInput04, createInput05
        };
        await CreateModelsShouldSucceed(createInput);

        var updateInput = new [] {
            new Models.Cast() { ActorId = createInput01.ActorId, MovieId = createInput01.MovieId, Value = "Value01_Changed" },
            new Models.Cast() { ActorId = createInput04.ActorId, MovieId = createInput04.MovieId, Value = "Value04_Changed" },
        };
        await UpdateModelsShouldSucceed(updateInput);
        
        var deleteInput = new [] {
            new Models.Cast() { ActorId = createInput02.ActorId, MovieId = createInput02.MovieId, Value = "SHOULDNOTCARE" },
        };
        await DeleteModelsShouldSucceed(deleteInput);

        var result = await QueryEntityShouldSucceed<Models.Cast>(
            new QueryRequest() {
                Filter = "value ne null", // keep a filter so Odata composite key proxy building is covered.
                Pagination = new Pagination() {
                    Page = 0,
                    PageSize = 10,
                },
                Sorting = new Sorting()
                {
                    Direction = SortingDirection.ASCENDING,
                    Field = nameof(Models.Cast.ActorId),
                }
            });
        var cast01 = result.Data.FirstOrDefault(x => x.ActorId == createInput01.ActorId && x.MovieId == createInput01.MovieId);
        cast01.Should().NotBeNull();
        cast01!.Value.Should().Be("Value01_Changed");
        var cast02 = result.Data.FirstOrDefault(x => x.ActorId == createInput02.ActorId && x.MovieId == createInput02.MovieId);
        cast02.Should().BeNull();
        var cast03 = result.Data.FirstOrDefault(x => x.ActorId == createInput03.ActorId && x.MovieId == createInput03.MovieId);
        cast03.Should().NotBeNull();
        cast03!.Value.Should().Be("Value03");
        var cast04 = result.Data.FirstOrDefault(x => x.ActorId == createInput04.ActorId && x.MovieId == createInput04.MovieId);
        cast04.Should().NotBeNull();
        cast04!.Value.Should().Be("Value04_Changed");
        var cast05 = result.Data.FirstOrDefault(x => x.ActorId == createInput05.ActorId && x.MovieId == createInput05.MovieId);
        cast05.Should().NotBeNull();
        cast05!.Value.Should().Be("Value05");        
    }
}
