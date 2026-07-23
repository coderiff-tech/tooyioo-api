using Tooyioo.User.Features;

namespace Tooyioo.Api.Infrastructure.EventStoreStateRetriever;

public static class EventStoreDetailsRetrieverExtensions
{
    public static TBuilder AddEventStoreDetailsRetrievers<TBuilder>(this TBuilder builder) where TBuilder 
        : IHostApplicationBuilder
    {
        builder.Services.AddTransient<IUserOnboardingDetailsRetriever, UserOnboardingDetailsRetriever>();
        return builder;
    }
}