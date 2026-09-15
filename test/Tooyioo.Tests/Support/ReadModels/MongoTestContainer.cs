using DotNet.Testcontainers.Builders;
using Testcontainers.MongoDb;
using TUnit.Core.Interfaces;

namespace Tooyioo.Tests.Support.ReadModels;

public sealed class MongoTestContainer
    : IAsyncDisposable
{
    private const string MongoImage = "mongo:8.3";

    private readonly Lock _lock = new();
    private MongoDbContainer? _container;
    private Task? _startTask;

    public async Task<MongoReadModelTestDatabase> CreateDatabase()
    {
        var connectionString = await GetConnectionString();

        return new MongoReadModelTestDatabase(connectionString, $"tooyioo_tests_{Guid.NewGuid():N}");
    }

    private async Task<string> GetConnectionString()
    {
        try
        {
            await EnsureStarted();
        }
        catch (DockerUnavailableException)
        {
            Skip.Test("Podman (or Docker) is required for query tests using Mongo");
            throw;
        }

        return _container!.GetConnectionString();
    }

    private Task EnsureStarted()
    {
        lock (_lock)
        {
            if (_startTask is not null)
            {
                return _startTask;
            }

            _container =
                new MongoDbBuilder(MongoImage)
                    // Keep glibc rseq enabled to prevent MongoDB's per-CPU TCMalloc cache on Linux kernels >= 6.19, where it cannot start reliably.
                    .WithEnvironment("GLIBC_TUNABLES", "glibc.pthread.rseq=1")
                    .Build();
            _startTask = _container.StartAsync();

            return _startTask;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}

public sealed record MongoReadModelTestDatabase(string ConnectionString, string DatabaseName);

public sealed class MongoReadModelParallelLimit
    : IParallelLimit
{
    public int Limit => 4;
}
