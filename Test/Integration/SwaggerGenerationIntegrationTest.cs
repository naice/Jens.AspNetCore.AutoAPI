namespace Test;

public class SwaggerGenerationIntegrationTest : AutoAPIIntegrationTestBase
{
    public SwaggerGenerationIntegrationTest(CustomWebApplicationFactory<Program> factory) : base(factory)
    {

    }

    [Fact]
    public async Task Ensure_Malforms_Failes()
    {
        var client = Factory.CreateClient();

        var swaggerJson = await client.GetStringAsync("/swagger/v1/swagger.json");
        swaggerJson.Should().NotBeNull();
    }
}