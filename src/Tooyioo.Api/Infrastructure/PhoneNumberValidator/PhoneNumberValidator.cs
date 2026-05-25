using PhoneNumbers;
using Tooyioo.Profile.Features.SetPhoneNumber.Support;
using PhoneNumberValidationResult = Tooyioo.Profile.Features.SetPhoneNumber.Support.PhoneNumberValidationResult;
// ReSharper disable ConvertConstructorToMemberInitializers

namespace Tooyioo.Api.Infrastructure.PhoneNumberValidator;

public class PhoneNumberValidator
    : IPhoneNumberValidator
{
    private readonly PhoneNumberUtil _phoneNumberUtil = PhoneNumberUtil.GetInstance();
    
    public PhoneNumberValidationResult IsValidE164(string phoneNumber)
    {
        if (!IsPlusPrefixedDigitsOnly(phoneNumber))
        {
            return new PhoneNumberValidationError("The phone number must start with a '+' and contain only digits");
        }

        try
        {
            var parsedNumber = _phoneNumberUtil.Parse(phoneNumber, defaultRegion: null); // parse as international
            var isValid = _phoneNumberUtil.IsValidNumber(parsedNumber);

            if (!isValid)
            {
                return new PhoneNumberValidationError("The phone number is not valid");
            }

            var isE164Canonical = _phoneNumberUtil.Format(parsedNumber, PhoneNumberFormat.E164) == phoneNumber;
            return !isE164Canonical
                ? new PhoneNumberValidationError("The phone number is not valid E.164 format")
                : PhoneNumberValidationResult.Ok();
        }
        catch (NumberParseException exception)
        {
            return new PhoneNumberValidationError($"The phone number is not valid, {exception.Message}");
        }
    }
    
    private static bool IsPlusPrefixedDigitsOnly(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return false;
        }

        var s = phoneNumber.AsSpan().Trim();
        if (s.Length < 2)
        {
            return false;
        }

        if (s[0] != '+')
        {
            return false;
        }

        for (var i = 1; i < s.Length; i++)
        {
            if (!char.IsDigit(s[i]))
            {
                return false;
            }
        }

        return true;
    }
}