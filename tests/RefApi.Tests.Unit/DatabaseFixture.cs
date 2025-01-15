using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using RefApi.Data;

namespace RefApi.Tests.Unit;

public class DatabaseFixture : IDisposable, IAsyncLifetime
{
    public SqliteConnection Connection { get; }
    public AppDbContext DbContext { get; }

    public DatabaseFixture()
    {
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(Connection)
            .Options;

        var userContext = new TestUserContext("test-user-id");
        DbContext = new AppDbContext(options, userContext);
    }

    public async Task ResetDatabase()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();
    }

    public Task InitializeAsync()
    {
        DbContext.Database.EnsureCreated();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public void Dispose()
    {
        DbContext.Dispose();
        Connection.Dispose();
    }
}