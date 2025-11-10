using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Nothing for now
        });
    }

    [Fact]
    public async Task Health_Returns_200()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/health");
        resp.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Swagger_And_Api_Endpoints_Require_ApiKey()
    {
        var client = _factory.CreateClient();
        // Swagger allowed
        var swagger = await client.GetAsync("/swagger/v1/swagger.json");
        swagger.EnsureSuccessStatusCode();

        // An API endpoint without key should be 401/403
        var resp = await client.GetAsync("/api/v1/usuarios");
        Assert.False(resp.IsSuccessStatusCode);

        // With API key
        client.DefaultRequestHeaders.Add("X-Api-Key", "dev-123456");
        var resp2 = await client.GetAsync("/api/v1/usuarios");
        // Could be success or 500 depending on external DB, but not 401/403
        Assert.NotEqual(System.Net.HttpStatusCode.Unauthorized, resp2.StatusCode);
        Assert.NotEqual(System.Net.HttpStatusCode.Forbidden, resp2.StatusCode);
    }
}
