// ReSharper disable UnusedType.Global
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tooyioo.Common;
using Tooyioo.Profile.Features.Bootstrap;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

namespace Tooyioo.Api.Infrastructure.Auth;

public static class AuthExtensions
{
    private const string DefaultScheme = "DefaultScheme";
    private const string BoostrapScheme = "BootstrapScheme";

    public static TBuilder AddAuth<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultForbidScheme = DefaultScheme;
                options.DefaultChallengeScheme = DefaultScheme;
            })
            .AddJwtBearer(DefaultScheme, options =>
            {
                ConfigureGoogleJwt(builder, options);
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnStandardTokenValidated,
                    OnChallenge = OnChallenge,
                    OnForbidden = OnForbidden
                };
            })
            .AddJwtBearer(BoostrapScheme, options =>
            {
                ConfigureGoogleJwt(builder, options);
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnBootstrapTokenValidated,
                    OnChallenge = OnChallenge,
                    OnForbidden = OnForbidden
                };
            })
            .Services
            .AddSingleton<IProfilePrincipalService, ProfilePrincipalCache>()
            .AddScoped<IPersonalDetailsRetriever, PersonalDetailsRetriever>()
            .AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(DefaultScheme)
                    .RequireAuthenticatedUser()
                    .Build())
            .AddPolicy(BootstrapProfileEndpoint.BootstrapAuthPolicy, p =>
                {
                    p.AddAuthenticationSchemes(BoostrapScheme);
                    p.RequireAuthenticatedUser();
                });
                
        return builder;
    }
    
    private static void ConfigureGoogleJwt(IHostApplicationBuilder builder, JwtBearerOptions options)
    {
        options.RequireHttpsMetadata = true;
        options.MapInboundClaims = false;
        options.Authority = "https://accounts.google.com";
        options.Audience = builder.Configuration["Google:ClientId"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = ["https://accounts.google.com", "accounts.google.com"],
            ValidateAudience = true,
            ValidateLifetime = true,
            NameClaimType = "sub"
        };
    }
    
    private static Task OnBootstrapTokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal!;

        var subOption = principal.GetSubClaim();
        if (!subOption.IsSome(out _))
        {
            context.Fail("JWT is missing the 'sub' claim required for bootstrap");
            return Task.CompletedTask;
        }
        
        var email = principal.FindFirstValue("email");
        if (string.IsNullOrWhiteSpace(email))
        {
            context.Fail("JWT is missing the 'email' claim required for bootstrap");
            return Task.CompletedTask;
        }
        
        var fullName  = principal.FindFirstValue("name");
        var givenName = principal.FindFirstValue("given_name");
        var lastName= principal.FindFirstValue("family_name");
        var isEmailVerified = bool.TryParse(principal.FindFirstValue("email_verified"), out var value) && value;
        
        HttpContextPersonalDetails.Set(
            context.HttpContext,
            new PersonalDetails(
                Name: givenName ?? fullName ?? string.Empty,
                LastName: lastName ?? string.Empty,
                Email: email,
                IsEmailVerified: isEmailVerified));
        
        return Task.CompletedTask;
    }
    
    private static async Task OnStandardTokenValidated(TokenValidatedContext context)
    {
        var profilePrincipalService = context.HttpContext.RequestServices.GetRequiredService<IProfilePrincipalService>();
        var principal = context.Principal!;
        var subOption = principal.GetSubClaim();
        if (!subOption.IsSome(out var sub))
        {
            context.Fail("JWT is missing the 'sub' claim");
            return;
        }

        var claimsPrincipal = await profilePrincipalService.GetByExternalId(sub);
        if (!claimsPrincipal.IsSome(out var claimsPrincipalValue))
        {
            context.Fail("Unknown user. Profile hasn't been bootstrapped yet.");
            return;
        }

        context.Principal = claimsPrincipalValue;
    }

    private static async Task OnForbidden(ForbiddenContext context)
    {
        var http = context.HttpContext;
        http.Response.StatusCode = StatusCodes.Status403Forbidden;

        await WriteProblemAsync(http, new ProblemDetails { Status = StatusCodes.Status403Forbidden, Title = "forbidden", Detail = "You are not allowed to access this resource." });
    }

    private static async Task OnChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse(); // suppress default 401

        var http = context.HttpContext;
        http.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await WriteProblemAsync(http, new ProblemDetails { Status = StatusCodes.Status401Unauthorized, Title = "unauthorized", Detail = "Missing or invalid bearer token." });
    }

    private static ValueTask WriteProblemAsync(HttpContext http, ProblemDetails problemDetails)
    {
        var service = http.RequestServices.GetRequiredService<IProblemDetailsService>();

        return service.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails = problemDetails
        });
    }
    
    // NEW
    private static Task OnGoogleTokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal!;
        var subOption = principal.GetSubClaim();
        if (!subOption.IsSome(out var sub))
        {
            context.Fail("JWT is missing the 'sub' claim");
            return Task.CompletedTask;
        }
    }
    
}