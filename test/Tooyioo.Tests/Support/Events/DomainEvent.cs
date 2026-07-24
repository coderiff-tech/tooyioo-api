using Tooyioo.Common;
using Tooyioo.Tests.Support;
using Tooyioo.Tests.Support.Extensions;
using Tooyioo.UserOnboarding;
using Tooyioo.UserOnboarding.Contracts;

namespace Tooyioo.Tests.Support.Events;

public static class DomainEvent
{
    public static UserOnboardingInitiatedBuilder UserOnboardingInitiated()
        => new();

    public static UserExternalIdentityAssociatedBuilder UserExternalIdentityAssociated()
        => new();

    public static UserEmailVerifiedBuilder UserEmailVerified()
        => new();

    public static UserAliasChosenBuilder UserAliasChosen()
        => new();

    public static UserOnboardingCompletedBuilder UserOnboardingCompleted()
        => new();

    public static UserExternalIdentityClaimedBuilder UserExternalIdentityClaimed()
        => new();

    public static UserAliasClaimedBuilder UserAliasClaimed()
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

public sealed class UserExternalIdentityAssociatedBuilder
{
    private string _id = "google-sub-123";
    private string _provider = nameof(ExternalIdentityProvider.Google);
    private string _issuer = GoogleIdentityTokenBuilder.Issuer;

    public UserExternalIdentityAssociatedBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public UserExternalIdentityAssociatedBuilder WithSubject(string subject)
    {
        _id = subject;
        return this;
    }

    public UserExternalIdentityAssociatedBuilder WithProvider(string provider)
    {
        _provider = provider;
        return this;
    }

    public UserExternalIdentityAssociatedBuilder WithIssuer(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    public UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated Build()
        => new(_id, _provider, _issuer);
}

public sealed class UserEmailVerifiedBuilder
{
    public UserOnboardingDomainEvents.V1.UserEmailVerified Build()
        => new();
}

public sealed class UserAliasChosenBuilder
{
    private string _alias = "jane-bloggs";

    public UserAliasChosenBuilder WithAlias(string alias)
    {
        _alias = alias;
        return this;
    }

    public UserOnboardingDomainEvents.V1.UserAliasChosen Build()
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

public sealed class UserExternalIdentityClaimedBuilder
{
    private string _userOnboardingId = 1.ToGuid().ToString();

    public UserExternalIdentityClaimedBuilder WithUserOnboardingId(string userOnboardingId)
    {
        _userOnboardingId = userOnboardingId;
        return this;
    }

    public UserExternalIdentityClaimedBuilder WithUserOnboardingId(UserOnboardingId userOnboardingId)
    {
        _userOnboardingId = userOnboardingId.Value;
        return this;
    }

    public UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed Build()
        => new(_userOnboardingId);
}

public sealed class UserAliasClaimedBuilder
{
    private string _userOnboardingId = 1.ToGuid().ToString();

    public UserAliasClaimedBuilder WithUserOnboardingId(string userOnboardingId)
    {
        _userOnboardingId = userOnboardingId;
        return this;
    }

    public UserAliasClaimedBuilder WithUserOnboardingId(UserOnboardingId userOnboardingId)
    {
        _userOnboardingId = userOnboardingId.Value;
        return this;
    }

    public UserAliasClaimingDomainEvents.V1.UserAliasClaimed Build()
        => new(_userOnboardingId);
}
