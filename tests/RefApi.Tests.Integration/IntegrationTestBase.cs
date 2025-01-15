using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using RefApi.Data;

using Testcontainers.PostgreSql;

namespace RefApi.Tests.Integration;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;

    private readonly PostgreSqlContainer _dbContainer;

    protected IntegrationTestBase()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace DbContext with test-specific options
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));

                    // Replace the default authentication scheme with a test scheme
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.TestScheme;
                        options.DefaultChallengeScheme = TestAuthenticationHandler.TestScheme;
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.TestScheme, _ => { });
                });
            });

        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}