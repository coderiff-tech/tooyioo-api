using Funzo;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.Complete.Support;

public interface IPhoneNumberValidator
{
    PhoneNumberValidationResult IsValidE164(string phoneNumber);
}

[Result<PhoneNumberValidationError>]
public partial class PhoneNumberValidationResult;

public sealed record PhoneNumberValidationError(string Reason);
