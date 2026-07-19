using Tooyioo.Api.Infrastructure.Auth;
using Tooyioo.Api.Infrastructure.Health;
using Tooyioo.Api.Infrastructure.MinimalApiValidator;
using Tooyioo.Api.Infrastructure.OpenApi;
using Tooyioo.Api.Infrastructure.Slicent;
using Tooyioo.Api.Infrastructure.Telemetry;

var builder = WebApplication.CreateBuilder(args);

var sliceAssemblies =
    new[]
    {
        typeof(Tooyioo.Profile.SliceAssemblyMarker).Assembly,
        typeof(Tooyioo.UserOnboarding.SliceAssemblyMarker).Assembly
    };

var domainEventAssemblies =
    new[]
    {
        typeof(Tooyioo.Profile.Contracts.ContractsAssemblyMarker).Assembly,
        typeof(Tooyioo.UserOnboarding.Contracts.ContractsAssemblyMarker).Assembly
    };

var allAssemblies = sliceAssemblies.Concat(domainEventAssemblies).ToArray();

#pragma warning disable ASP0029
builder
    .AddHealth()
    .AddTelemetry()
    .AddOpenApi()
    //.AddPhoneNumberValidator()
    .AddMinimalApiValidator(sliceAssemblies)
    .AddSlicent(allAssemblies)
    .AddAuth();

#pragma warning restore ASP0029

var app = builder.Build();

app.Logger.LogInformation("Application is starting..");

app
    .UseOpenApi()
    .UseSlicent()
    .UseHealth();

app.Run();