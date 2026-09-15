using System.Reflection;
using Eventuous.KurrentDB.Subscriptions;
using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Registrations;
using KurrentDB.Client;
using Slicent;
using Slicent.Application;
using Tooyioo.Api.Infrastructure.OpenApi;
using Tooyioo.User.Features.Support;
using Tooyioo.UserOnboarding.Features.RetrieveUserOnboarding.Support;

namespace Tooyioo.Api.Infrastructure.Slicent;

public static class SlicentExtensions
{
    public static TBuilder AddSlicent<TBuilder>(this TBuilder builder, params Assembly[] assemblies) 
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddSlicent(assemblies);

        if (builder.Configuration.GetValue<bool>("Slicent:IsTestExecution")
            || Assembly.GetEntryAssembly().IsOpenApiGenerationLaunch())
        {
            return builder;
        }

        var kurrentDbConnectionString =
            builder.Configuration.GetConnectionString("KurrentDb")
            ?? throw new InvalidOperationException("KurrentDb connection string is not set");
        
        var mongoDbConnectionString =
            builder.Configuration.GetConnectionString("MongoDb")
            ?? throw new InvalidOperationException("MongoDb connection string is not set");
        
        builder.Services
            .AddSlicentKurrentDb(kurrentDbConnectionString)
            .AddSlicentMongoDb(mongoDbConnectionString)
            .AddSubscription<AllStreamSubscription, AllStreamSubscriptionOptions>(
                "ReadModelsSubscription",
                subscriptionBuilder => subscriptionBuilder
                    .Configure(configureOptions =>
                    {
                        configureOptions.ThrowOnError = true;
                        configureOptions.EventFilter = EventTypeFilter.ExcludeSystemEvents();
                        configureOptions.CheckpointInterval = 10;
                    })
                    .UseCheckpointStore<MongoCheckpointStore>()
                    .AddEventHandler<UserOnboardingProjector>()
                    .AddEventHandler<UserProjector>());
            
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
