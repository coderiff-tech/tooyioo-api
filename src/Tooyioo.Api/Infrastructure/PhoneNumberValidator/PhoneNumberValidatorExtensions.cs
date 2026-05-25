using Tooyioo.Profile.Features.SetPhoneNumber.Support;

// ReSharper disable UnusedType.Global

namespace Tooyioo.Api.Infrastructure.PhoneNumberValidator;

public static class PhoneNumberValidatorExtensions
{
    extension<TBuilder>(TBuilder builder) where TBuilder 
        : IHostApplicationBuilder
    {
        public TBuilder AddPhoneNumberValidator()
        {
            builder.Services.AddSingleton<IPhoneNumberValidator, PhoneNumberValidator>();
            return builder;
        }
    }
}