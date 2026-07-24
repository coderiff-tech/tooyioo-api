using System.Net;
using Tooyioo.Common;
using Tooyioo.Tests.Support;
using Tooyioo.Tests.Support.Extensions;
using Tooyioo.Tests.Support.Http;
using Tooyioo.Tests.Support.ReadModels;
using Tooyioo.User.Features.RetrieveUser.Contract;
using Tooyioo.User.Features.Support;
using Tooyioo.UserOnboarding;
using Tooyioo.UserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

namespace Tooyioo.Tests.Features.RetrieveUser;

public sealed class WhenUserExists(MongoReadModelTestContainer mongoDb)
    : MongoReadModelVerticalSliceGivenWhenThen(mongoDb)
{
    private readonly DateTime _createdAtUtc = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private HttpResponseMessage _response = null!;
    private RetrieveUserResponse _body = null!;
    private UserOnboardingId _userOnboardingId = null!;
    private string _userId = null!;
    private string _subject = null!;
    private string _termsAndConditionsVersion = null!;

    protected override async Task Given()
    {
        _userOnboardingId = new UserOnboardingId(1.ToGuid().ToString());
        _userId = 2.ToGuid().ToString();
        _subject = "google-sub-123";
        _termsAndConditionsVersion = "v1";

        await Host.Given<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
            new ClaimingExternalIdentityId(_subject),
            new UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed(_userOnboardingId));

        await Host.GivenAndProject<UserProjector, UserOnboardingState, UserOnboardingId>(
            _userOnboardingId,
            _createdAtUtc,
            new UserOnboardingDomainEvents.V1.UserOnboardingInitiated(
                "Jane",
                "Bloggs",
                "jane.bloggs@test.com"),
            new UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated(
                _subject,
                nameof(ExternalIdentityProvider.Google),
                GoogleIdentityTokenBuilder.Issuer),
            new UserOnboardingDomainEvents.V1.UserEmailVerified(),
            new UserOnboardingDomainEvents.V1.UserAliasChosen("jane-bloggs"),
            new UserOnboardingDomainEvents.V1.UserOnboardingCompleted(
                _userId,
                _termsAndConditionsVersion));
    }

    protected override async Task When()
    {
        var token = Host.GoogleIdentityToken()
            .WithSubject(_subject)
            .Build();

        _response = await Host.HttpClient.Get($"/users/{_userId}", token);
        _body = await _response.ReadJson<RetrieveUserResponse>();
    }

    [Test]
    public async Task Then_response_is_ok()
        => await Assert.That(_response.StatusCode).IsEqualTo(HttpStatusCode.OK);

    [Test]
    public async Task Then_response_returns_user_id()
        => await Assert.That(_body.Id).IsEqualTo(_userId);

    [Test]
    public async Task Then_response_returns_projected_personal_details()
    {
        await Assert.That(_body.Name).IsEqualTo("Jane");
        await Assert.That(_body.LastName).IsEqualTo("Bloggs");
        await Assert.That(_body.Email).IsEqualTo("jane.bloggs@test.com");
        await Assert.That(_body.IsEmailVerified).IsTrue();
        await Assert.That(_body.Alias).IsEqualTo("jane-bloggs");
    }

    [Test]
    public async Task Then_response_returns_projected_onboarding_details()
    {
        await Assert.That(_body.ExternalId).IsEqualTo(_subject);
        await Assert.That(_body.ExternalProvider).IsEqualTo(nameof(ExternalIdentityProvider.Google));
        await Assert.That(_body.UserOnboardingId).IsEqualTo(_userOnboardingId.Value);
        await Assert.That(_body.TermsAndConditionsVersion).IsEqualTo(_termsAndConditionsVersion);
    }

    [Test]
    public async Task Then_response_returns_projection_timestamps()
    {
        await Assert.That(_body.CreatedAt).IsEqualTo(_createdAtUtc);
        await Assert.That(_body.LastModifiedAt).IsEqualTo(_createdAtUtc);
    }
}
