using Tooiyoo.Api.Infrastructure.Auth;
using Tooiyoo.Api.Infrastructure.Health;
using Tooiyoo.Api.Infrastructure.MinimalApiValidator;
using Tooiyoo.Api.Infrastructure.OpenApi;
using Tooiyoo.Api.Infrastructure.PhoneNumberValidator;
using Tooiyoo.Api.Infrastructure.Slicent;
using Tooiyoo.Api.Infrastructure.Telemetry;

var builder = WebApplication.CreateBuilder(args);

var sliceAssemblies =
    new[]
    {
        typeof(Tooiyoo.Identity.SliceAssemblyMarker).Assembly,
        typeof(Tooyioo.Profile.SliceAssemblyMarker).Assembly
    };

var domainEventAssemblies =
    new[]
    {
        typeof(Tooiyoo.Identity.Contracts.ContractsAssemblyMarker).Assembly
    };

var allAssemblies = sliceAssemblies.Concat(domainEventAssemblies).ToArray();

#pragma warning disable ASP0029
builder
    .AddHealth()
    .AddTelemetry()
    .AddOpenApi()
    .AddPhoneNumberValidator()
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