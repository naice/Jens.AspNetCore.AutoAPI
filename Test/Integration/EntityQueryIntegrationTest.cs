using Jens.AspNetCore.AutoAPI.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Test;

public class EntityQueryIntegrationTest : AutoAPIIntegrationTestBase
{
    public EntityQueryIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_EntityQuery_Order_Filter_Pagination_Sort_Succeed()
    {
        await CreateActorsShouldSucceed(5, "XYZ");
        await CreateActorsShouldSucceed(5, "name");

        const string filter = "startswith(name, 'name')";
        var sorting = new Sorting() {
            Direction = SortingDirection.DESCENDING,
            Field = "name"
        };

        var result = await QueryEntityShouldSucceed<Models.Actor>(
            new QueryRequest() {
                Filter = filter,
                Pagination = new Pagination() {
                    Page = 1,
                    PageSize = 3,
                },
                Sorting = sorting
            });

        result.Filter.Should().Be(filter);
        result.Sorting.Should().NotBeNull();
        result.Sorting.Direction.Should().Be(sorting.Direction);
        result.Sorting.Field.Should().Be(sorting.Field);
        result.Pagination.Should().NotBeNull();
        result.Pagination.Page.Should().Be(1);
        result.Pagination.PageSize.Should().Be(3);
        result.Pagination.Total.Should().Be(5);

        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("name1");
        result.Data[1].Name.Should().Be("name0");        
    }
}
