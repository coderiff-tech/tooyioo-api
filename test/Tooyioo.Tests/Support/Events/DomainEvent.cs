using Tooyioo.Common;
using Tooyioo.Tests.Support.Extensions;
using Tooyioo.UserOnboarding;
using Tooyioo.UserOnboarding.Contracts;

namespace Tooyioo.Tests.Support.Events;

public static class DomainEvent
{
    public static UserOnboardingInitiatedBuilder UserOnboardingInitiated()
        => new();

    public static UserOnboardingExternalIdentityAssociatedBuilder UserOnboardingExternalIdentityAssociated()
        => new();

    public static UserOnboardingEmailVerifiedBuilder UserOnboardingEmailVerified()
        => new();

    public static UserOnboardingAliasChosenBuilder UserOnboardingAliasChosen()
        => new();

    public static UserOnboardingCompletedBuilder UserOnboardingCompleted()
        => new();

    public static UserOnboardingCanceledBuilder UserOnboardingCanceled()
        => new();

    public static ExternalIdentityClaimedBuilder ExternalIdentityClaimed()
        => new();

    public static ExternalIdentityReleasedBuilder ExternalIdentityReleased()
        => new();

    public static AliasClaimedBuilder AliasClaimed()
        => new();

    public static AliasReleasedBuilder AliasReleased()
        => new();
}

public sealed class UserOnboardingInitiatedBuilder
{
    private string _name = "Jane";
    private string _lastName = "Bloggs";
    private string _email = "jane.bloggs@test.com";

    public UserOnboardingInitiatedBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserOnboardingInitiatedBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UserOnboardingInitiatedBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserOnboardingDomainEvents.V1.UserOnboardingInitiated Build()
        => new(_name, _lastName, _email);
}

public sealed class UserOnboardingExternalIdentityAssociatedBuilder
{
    private string _id = "google-sub-123";
    private string _provider = nameof(ExternalIdentityProvider.Google);
    private string _issuer = GoogleIdentityTokenBuilder.Issuer;

    public UserOnboardingExternalIdentityAssociatedBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public UserOnboardingExternalIdentityAssociatedBuilder WithSubject(string subject)
    {
        _id = subject;
        return this;
    }

    public UserOnboardingExternalIdentityAssociatedBuilder WithProvider(string provider)
    {
        _provider = provider;
        return this;
    }

    public UserOnboardingExternalIdentityAssociatedBuilder WithIssuer(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    public UserOnboardingDomainEvents.V1.UserOnboardingExternalIdentityAssociated Build()
        => new(_id, _provider, _issuer);
}

public sealed class UserOnboardingEmailVerifiedBuilder
{
    public UserOnboardingDomainEvents.V1.UserOnboardingEmailVerified Build()
        => new();
}

public sealed class UserOnboardingAliasChosenBuilder
{
    private string _alias = "jane-bloggs";

    public UserOnboardingAliasChosenBuilder WithAlias(string alias)
    {
        _alias = alias;
        return this;
    }

    public UserOnboardingDomainEvents.V1.UserOnboardingAliasChosen Build()
        => new(_alias);
}

public sealed class UserOnboardingCompletedBuilder
{
    private string _userId = 1.ToGuid().ToString();
    private string _termsAndConditionsVersion = "v1";

    public UserOnboardingCompletedBuilder WithUserId(string userId)
    {
        _userId = userId;
        return this;
    }

    public UserOnboardingCompletedBuilder WithTermsAndConditionsVersion(string termsAndConditionsVersion)
    {
        _termsAndConditionsVersion = termsAndConditionsVersion;
        return this;
    }

    public UserOnboardingDomainEvents.V1.UserOnboardingCompleted Build()
        => new(_userId, _termsAndConditionsVersion);
}

public sealed class UserOnboardingCanceledBuilder
{
    private string _reason = "Terms and Conditions rejected";

    public UserOnboardingCanceledBuilder WithReason(string reason)
    {
        _reason = reason;
        return this;
    }

    public UserOnboardingDomainEvents.V1.UserOnboardingCanceled Build()
        => new(_reason);
}

public sealed class ExternalIdentityClaimedBuilder
{
    private string _userOnboardingId = 1.ToGuid().ToString();

    public ExternalIdentityClaimedBuilder WithUserOnboardingId(string userOnboardingId)
    {
        _userOnboardingId = userOnboardingId;
        return this;
    }

    public ExternalIdentityClaimedBuilder WithUserOnboardingId(UserOnboardingId userOnboardingId)
    {
        _userOnboardingId = userOnboardingId.Value;
        return this;
    }

    public ExternalIdentityClaimingDomainEvents.V1.ExternalIdentityClaimed Build()
        => new(_userOnboardingId);
}

public sealed class ExternalIdentityReleasedBuilder
{
    private string _userOnboardingId = 1.ToGuid().ToString();

    public ExternalIdentityReleasedBuilder WithUserOnboardingId(string userOnboardingId)
    {
        _userOnboardingId = userOnboardingId;
        return this;
    }

    public ExternalIdentityReleasedBuilder WithUserOnboardingId(UserOnboardingId userOnboardingId)
    {
        _userOnboardingId = userOnboardingId.Value;
        return this;
    }

    public ExternalIdentityClaimingDomainEvents.V1.ExternalIdentityReleased Build()
        => new(_userOnboardingId);
}

public sealed class AliasClaimedBuilder
{
    private string _userOnboardingId = 1.ToGuid().ToString();

    public AliasClaimedBuilder WithUserOnboardingId(string userOnboardingId)
    {
        _userOnboardingId = userOnboardingId;
        return this;
    }

    public AliasClaimedBuilder WithUserOnboardingId(UserOnboardingId userOnboardingId)
    {
        _userOnboardingId = userOnboardingId.Value;
        return this;
    }

    public AliasClaimingDomainEvents.V1.AliasClaimed Build()
        => new(_userOnboardingId);
}

public sealed class AliasReleasedBuilder
{
    private string _userOnboardingId = 1.ToGuid().ToString();

    public AliasReleasedBuilder WithUserOnboardingId(string userOnboardingId)
    {
        _userOnboardingId = userOnboardingId;
        return this;
    }

    public AliasReleasedBuilder WithUserOnboardingId(UserOnboardingId userOnboardingId)
    {
        _userOnboardingId = userOnboardingId.Value;
        return this;
    }

    public AliasClaimingDomainEvents.V1.AliasReleased Build()
        => new(_userOnboardingId);
}
