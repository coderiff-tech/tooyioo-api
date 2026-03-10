using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.Profile.Features.Complete.Support;

public sealed class InternationalPhoneNumberAttribute
    : ValidationAttribute
{
    public InternationalPhoneNumberAttribute(string errorMessage)
        : base(errorMessage)
    { }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is null)
        {
            // assuming other handlers deal with null values
            return ValidationResult.Success;
        }
        
        var phoneNumberValidator = context.GetRequiredService<IPhoneNumberValidator>();
        var phoneNumber = value.ToString() ?? string.Empty;
        var phoneNumberValidatorResult = phoneNumberValidator.IsValidE164(phoneNumber);
        var validationResult =
            phoneNumberValidatorResult.Match(
                () => ValidationResult.Success,
                _ => new ValidationResult(ErrorMessageString, [context.MemberName!]));
        return validationResult;
    }
}
