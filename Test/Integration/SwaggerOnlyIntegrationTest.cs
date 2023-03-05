using Jens.AspNetCore.AutoAPI;
using Microsoft.Extensions.Logging.Abstractions;

namespace Test;

public class SwaggerOnlyUnitTest
{
    public const string SWAGGER_FILE_PATH = "TestResults\\swagger.yaml";

    [Fact]
    public void TestSwaggerGeneration()
    {
        // TODO:

        // var mockConfig = new Mock<IConfiguration>();
        // mockConfig.Setup(x => x["swaggeronly"]).Returns(SWAGGER_FILE_PATH);
        // var mockHost = new Mock<IHost>();
        // var mockServiceProvider = new Mock<IServiceProvider>();
        // mockServiceProvider.Setup(x => x.GetService(typeof(IConfiguration))).Returns(mockConfig.Object);
        // mockServiceProvider.Setup(x => x.GetService(typeof(ILogger))).Returns(
        //     NullLoggerProvider.Instance.CreateLogger(string.Empty)
        // );
        // mockHost.Setup(x => x.Services).Returns(mockServiceProvider.Object);

        // SwaggerExtensions.TryRunSwaggerOutputOnly(mockHost.Object).Should().BeTrue();
    }
}
