using System.Reflection;
using Eventuous.KurrentDB.Subscriptions;
using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Registrations;
using Slicent;
using Slicent.Application;
using Tooiyoo.Api.Infrastructure.OpenApi;
using Tooiyoo.Identity.ReadModel;

namespace Tooiyoo.Api.Infrastructure.Slicent;

public static class SlicentExtensions
{
    public static TBuilder AddSlicent<TBuilder>(this TBuilder builder, params Assembly[] assemblies) 
        where TBuilder : IHostApplicationBuilder
    {
        var kurrentDbConnectionString =
            builder.Configuration.GetConnectionString("KurrentDb")
            ?? throw new InvalidOperationException("KurrentDb connection string is not set");
        
        var mongoDbConnectionString =
            builder.Configuration.GetConnectionString("MongoDb")
            ?? throw new InvalidOperationException("MongoDb connection string is not set");

        builder.Services.AddSlicent(assemblies);
            
        if (Assembly.GetEntryAssembly().IsOpenApiGenerationLaunch())
        {
            return builder;
        }
        
        builder.Services
            .AddSlicentKurrentDb(kurrentDbConnectionString)
            .AddSlicentMongoDb(mongoDbConnectionString)
            .AddSubscription<AllStreamSubscription, AllStreamSubscriptionOptions>(
                "ReadModelsSubscription",
                subscriptionBuilder => subscriptionBuilder
                    .UseCheckpointStore<MongoCheckpointStore>()
                    .AddEventHandler<ProfileProjector>());
            
        return builder;
    }

    public static WebApplication UseSlicent(this WebApplication app)
    {
        foreach (var module in app.Services.GetServices<IHttpEndpointModule>())
        {
            module.Map(app);
        }
        
        return app;
    }
}