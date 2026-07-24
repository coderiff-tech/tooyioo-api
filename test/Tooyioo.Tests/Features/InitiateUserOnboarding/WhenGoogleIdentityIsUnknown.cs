using System.Net;
using Tooyioo.Common;
using Tooyioo.Tests.Support;
using Tooyioo.Tests.Support.Http;
using Tooyioo.Tests.Support.VerticalSlices;
using Tooyioo.UserOnboarding;
using Tooyioo.UserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

namespace Tooyioo.Tests.Features.InitiateUserOnboarding;

public sealed class WhenGoogleIdentityIsUnknown
    : CommandVerticalSliceTest
{
    private HttpResponseMessage _response = null!;
    private InitiateUserOnboardingResponse _body = null!;
    private string _subject = null!;
    private string _name = null!;
    private string _lastName = null!;
    private string _email = null!;

    protected override Task Given()
        => Task.CompletedTask;

    protected override async Task When()
    {
        _subject = "google-sub-123";
        _name = "Jane";
        _lastName = "Bloggs";
        _email = "jane.bloggs@test.com";
        
        var token = Host.GoogleIdentityToken()
            .WithSubject(_subject)
            .WithPersonalDetails(_name, _lastName, _email, true)
            .Build();

        _response = await Host.HttpClient.PostJson(
            "/user-onboarding/initiate",
            new { },
            token);

        _body = await _response.ReadJson<InitiateUserOnboardingResponse>();
    }

    [Test]
    public async Task Then_response_is_ok()
        => await Assert.That(_response.StatusCode).IsEqualTo(HttpStatusCode.OK);

    [Test]
    public async Task Then_response_contains_user_onboarding_id()
        => await Assert.That(Guid.TryParse(_body.UserOnboardingId, out _)).IsTrue();

    [Test]
    public async Task Then_user_onboarding_events_are_saved()
        => await Host.Events
            .Stream<UserOnboardingState, UserOnboardingId>(new UserOnboardingId(_body.UserOnboardingId))
            .ShouldContainExactly(
                new UserOnboardingDomainEvents.V1.UserOnboardingInitiated(_name, _lastName,_email),
                new UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated(
                    _subject,
                    nameof(ExternalIdentityProvider.Google),
                    GoogleIdentityTokenBuilder.Issuer),
                new UserOnboardingDomainEvents.V1.UserEmailVerified());

    [Test]
    public async Task Then_external_identity_is_claimed()
        => await Host.Events
            .Stream<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(new ClaimingExternalIdentityId(_subject))
            .ShouldContainExactly(
                new UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed(_body.UserOnboardingId));
}
