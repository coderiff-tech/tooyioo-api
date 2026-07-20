using Eventuous;
using Eventuous.Testing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tooyioo.Tests.Support.Auth;

namespace Tooyioo.Tests.Support;

internal sealed class VerticalSliceWebApplicationFactory(
    InMemoryEventStore eventStore,
    byte[] signingKey,
    Action<IServiceCollection>? overrideServices)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:KurrentDb"] = "kurrentdb://localhost:2113?tls=false",
                ["ConnectionStrings:MongoDb"] = "mongodb://127.0.0.1:27017",
                ["Google:ClientId"] = "tooyioo-tests"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHostedService>();

            services.RemoveAll<IEventStore>();
            services.RemoveAll<IEventReader>();
            services.RemoveAll<IEventWriter>();
            services.AddSingleton<IEventStore>(eventStore);
            services.AddSingleton<IEventReader>(eventStore);
            services.AddSingleton<IEventWriter>(eventStore);

            services.RemoveAll<IMongoDatabase>();
            services.AddSingleton<IMongoDatabase>(_ =>
                new MongoClient("mongodb://127.0.0.1:27017").GetDatabase("TooyiooVerticalSliceTests"));

            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>>(new TestJwtBearerOptions(signingKey));

            overrideServices?.Invoke(services);
        });
    }
}
