using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Michi.API.IntegrationTests;

public sealed class HealthTests(MichiWebApplicationFactory factory)
    : IClassFixture<MichiWebApplicationFactory>
{
    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task HealthStatus_ShouldReturnHttp200(string endpoint)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_InDevelopment_ShouldReturnHttp200()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_InProduction_ShouldReturnNotFound()
    {
        using var prodFactory = factory.WithWebHostBuilder(b =>
            b.UseEnvironment(Environments.Production));

        var client = prodFactory.CreateClient();
        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}