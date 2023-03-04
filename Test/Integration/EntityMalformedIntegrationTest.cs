namespace Test;

public class EntityMalformedIntegrationTest : AutoAPIIntegrationTestBase
{
    public EntityMalformedIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_Malforms_Failes()
    {
        await QueryActorMalformedShouldFail();
        await CreateActorMalformedShouldFail();
        await DeleteActorMalformedShouldFail();
        await UpdateActorMalformedShouldFail();
        await CreateActorsMalformedShouldFail();
        await DeleteActorsMalformedShouldFail();
        await UpdateActorsMalformedShouldFail();
        await CreateOrUpdateActorMalformedShouldFail();
        await CreateOrUpdateListActorsMalformedShouldFail();
    }
}
