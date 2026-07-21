using System.Net;
using Tooyioo.Common;
using Tooyioo.Tests.Support;
using Tooyioo.Tests.Support.Events;
using Tooyioo.Tests.Support.Extensions;
using Tooyioo.Tests.Support.Http;
using Tooyioo.UserOnboarding;
using Tooyioo.UserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.CompleteUserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

namespace Tooyioo.Tests.Features.CompleteUserOnboarding;

public sealed class WhenUserOnboardingCanBeCompleted
    : VerticalSliceGivenWhenThen
{
    private HttpResponseMessage _response = null!;
    private CompleteUserOnboardingResponse _body = null!;
    private UserOnboardingId _userOnboardingId = null!;
    private string _subject = null!;
    private string _termsAndConditionsVersion = null!;
    private EventStreamSnapshot _userOnboardingStreamBeforeWhen = null!;

    protected override async Task Given()
    {
        _userOnboardingId = new UserOnboardingId(1.ToGuid().ToString());
        _subject = "google-sub-123";
        _termsAndConditionsVersion = "v1";

        await Host.Given<UserOnboardingState, UserOnboardingId>(
            _userOnboardingId,
            new UserOnboardingDomainEvents.V1.UserOnboardingInitiated(
                "Jane",
                "Bloggs",
                "jane.bloggs@test.com"),
            new UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated(
                _subject,
                nameof(ExternalIdentityProvider.Google),
                GoogleIdentityTokenBuilder.Issuer),
            new UserOnboardingDomainEvents.V1.UserEmailVerified(),
            new UserOnboardingDomainEvents.V1.UserAliasChosen("jane-bloggs"));

        await Host.Given<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
            new ClaimingExternalIdentityId(_subject),
            new UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed(_userOnboardingId));

        _userOnboardingStreamBeforeWhen = await Host.Events
            .Stream<UserOnboardingState, UserOnboardingId>(_userOnboardingId)
            .CaptureSnapshot();
    }

    protected override async Task When()
    {
        var token = Host.GoogleIdentityToken()
            .WithSubject(_subject)
            .Build();

        _response = await Host.HttpClient.PostJson(
            $"/user-onboarding/{_userOnboardingId.Value}/complete",
            new CompleteUserOnboardingRequest { TermsAndConditionsVersion = _termsAndConditionsVersion },
            token);

        _body = await _response.ReadJson<CompleteUserOnboardingResponse>();
    }

    [Test]
    public async Task Then_response_is_ok()
        => await Assert.That(_response.StatusCode).IsEqualTo(HttpStatusCode.OK);

    [Test]
    public async Task Then_response_returns_user_onboarding_id()
        => await Assert.That(_body.UserOnboardingId).IsEqualTo(_userOnboardingId.Value);

    [Test]
    public async Task Then_response_returns_user_id()
        => await Assert.That(Guid.TryParse(_body.UserId, out _)).IsTrue();

    [Test]
    public async Task Then_user_onboarding_is_completed()
        => await Host.Events
            .Stream<UserOnboardingState, UserOnboardingId>(_userOnboardingId)
            .ShouldAppendExactly(
                _userOnboardingStreamBeforeWhen,
                new UserOnboardingDomainEvents.V1.UserOnboardingCompleted(
                    _body.UserId,
                    _termsAndConditionsVersion));
}
