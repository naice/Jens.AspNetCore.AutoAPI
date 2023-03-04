using Jens.AspNetCore.AutoAPI;

namespace Test;

public class EntityDbContextTypeConfigurationTest
{

    [Fact]
    public void Ensure_Default_Pick()
    {
        var config = new EntityDbContextTypeConfiguration<TestContext0>()
            .With<TestContext1, Models.Movie>();
            
        config.GetDbContext(typeof(Models.Actor)).Should().Be(typeof(TestContext0));
        config.GetDbContext(typeof(Models.Movie)).Should().Be(typeof(TestContext1));
    }
}
