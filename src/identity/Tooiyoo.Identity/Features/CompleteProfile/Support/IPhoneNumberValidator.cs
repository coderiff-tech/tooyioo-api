using Funzo;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Features.CompleteProfile.Support;

public interface IPhoneNumberValidator
{
    PhoneNumberValidationResult IsValidE164(string phoneNumber);
}

[Result<PhoneNumberValidationError>]
public partial class PhoneNumberValidationResult;

public sealed record PhoneNumberValidationError(string Reason);
