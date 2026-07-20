using System.Net;
using Slicent.Application;
using Tooyioo.Common;
using Tooyioo.Tests.Support;
using Tooyioo.Tests.Support.Extensions;
using Tooyioo.Tests.Support.Http;
using Tooyioo.UserOnboarding;
using Tooyioo.UserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Contracts;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

namespace Tooyioo.Tests.Features.ChooseUserAlias;

public sealed class WhenAnotherUserTriesToChooseAliasForUserOnboarding
    : VerticalSliceGivenWhenThen
{
    private HttpResponseMessage _response = null!;
    private HttpProblemDetails _body = null!;
    private UserOnboardingId _ownerUserOnboardingId = null!;
    private UserOnboardingId _anotherUserOnboardingId = null!;
    private string _ownerSubject = null!;
    private string _anotherSubject = null!;
    private string _alias = null!;

    protected override async Task Given()
    {
        _ownerUserOnboardingId = new UserOnboardingId(1.ToGuid().ToString());
        _anotherUserOnboardingId = new UserOnboardingId(2.ToGuid().ToString());
        _ownerSubject = "owner-google-sub-123";
        _anotherSubject = "another-google-sub-456";
        _alias = "jane-bloggs";

        await Host.Given<UserOnboardingState, UserOnboardingId>(
            _ownerUserOnboardingId,
            new UserOnboardingDomainEvents.V1.UserOnboardingInitiated(
                "Jane",
                "Bloggs",
                "jane.bloggs@test.com"),
            new UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated(
                _ownerSubject,
                nameof(ExternalIdentityProvider.Google),
                GoogleIdentityTokenBuilder.Issuer),
            new UserOnboardingDomainEvents.V1.UserEmailVerified());

        await Host.Given<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
            new ClaimingExternalIdentityId(_ownerSubject),
            new UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed(_ownerUserOnboardingId));

        await Host.Given<UserOnboardingState, UserOnboardingId>(
            _anotherUserOnboardingId,
            new UserOnboardingDomainEvents.V1.UserOnboardingInitiated(
                "Other",
                "User",
                "other.user@test.com"),
            new UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated(
                _anotherSubject,
                nameof(ExternalIdentityProvider.Google),
                GoogleIdentityTokenBuilder.Issuer),
            new UserOnboardingDomainEvents.V1.UserEmailVerified());

        await Host.Given<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
            new ClaimingExternalIdentityId(_anotherSubject),
            new UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed(_anotherUserOnboardingId));
    }

    protected override async Task When()
    {
        var anotherUserToken = Host.GoogleIdentityToken()
            .WithSubject(_anotherSubject)
            .WithPersonalDetails("Other", "User", "other.user@test.com", true)
            .Build();

        _response = await Host.HttpClient.PostJson(
            $"/user-onboarding/{_ownerUserOnboardingId.Value}/choose-alias",
            new ChooseUserAliasRequest { Alias = _alias },
            anotherUserToken);

        _body = await _response.ReadJson<HttpProblemDetails>();
    }

    [Test]
    public async Task Then_response_is_forbidden()
        => await Assert.That(_response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);

    [Test]
    public async Task Then_response_title_is_forbidden()
        => await Assert.That(_body.Title).IsEqualTo("forbidden");

    [Test]
    public async Task Then_response_explains_user_is_not_allowed()
        => await Assert.That(_body.Detail).IsEqualTo("You are not allowed to access this resource.");
}
