using System.Net;
using Tooyioo.Tests.Support;
using Tooyioo.Tests.Support.Events;
using Tooyioo.Tests.Support.Extensions;
using Tooyioo.Tests.Support.Http;
using Tooyioo.Tests.Support.VerticalSlices;
using Tooyioo.UserOnboarding;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Contracts;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Support;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

namespace Tooyioo.Tests.Features.ChooseUserAlias;

public sealed class WhenUserAliasIsAvailable
    : CommandVerticalSliceTest
{
    private HttpResponseMessage _response = null!;
    private ChooseUserAliasResponse _body = null!;
    private UserOnboardingId _userOnboardingId = null!;
    private string _subject = null!;
    private string _name = null!;
    private string _lastName = null!;
    private string _email = null!;
    private string _alias = null!;
    private EventStreamSnapshot _userOnboardingStreamBeforeWhen = null!;
    private EventStreamSnapshot _userAliasClaimStreamBeforeWhen = null!;

    protected override async Task Given()
    {
        _userOnboardingId = new UserOnboardingId(1.ToGuid().ToString());
        _subject = "google-sub-123";
        _name = "Jane";
        _lastName = "Bloggs";
        _email = "jane.bloggs@test.com";
        _alias = "jane-bloggs";

        await Host.Given<UserOnboardingState, UserOnboardingId>(
            _userOnboardingId,
            DomainEvent.UserOnboardingInitiated()
                .WithName(_name)
                .WithLastName(_lastName)
                .WithEmail(_email)
                .Build(),
            DomainEvent.UserExternalIdentityAssociated()
                .WithSubject(_subject)
                .Build(),
            DomainEvent.UserEmailVerified().Build());

        await Host.Given<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
            new ClaimingExternalIdentityId(_subject),
            DomainEvent.UserExternalIdentityClaimed()
                .WithUserOnboardingId(_userOnboardingId)
                .Build());

        _userOnboardingStreamBeforeWhen = await Host.Events
            .Stream<UserOnboardingState, UserOnboardingId>(_userOnboardingId)
            .CaptureSnapshot();

        _userAliasClaimStreamBeforeWhen = await Host.Events
            .Stream<ClaimingUserAliasState, ClaimingUserAliasId>(new ClaimingUserAliasId(_alias))
            .CaptureSnapshot();
    }

    protected override async Task When()
    {
        var token = Host.GoogleIdentityToken()
            .WithSubject(_subject)
            .WithPersonalDetails(_name, _lastName, _email, true)
            .Build();

        _response = await Host.HttpClient.PostJson(
            $"/user-onboarding/{_userOnboardingId.Value}/choose-alias",
            new ChooseUserAliasRequest { Alias = _alias },
            token);

        _body = await _response.ReadJson<ChooseUserAliasResponse>();
    }

    [Test]
    public async Task Then_response_is_ok()
        => await Assert.That(_response.StatusCode).IsEqualTo(HttpStatusCode.OK);

    [Test]
    public async Task Then_response_returns_user_onboarding_id()
        => await Assert.That(_body.UserOnboardingId).IsEqualTo(_userOnboardingId.Value);

    [Test]
    public async Task Then_user_onboarding_alias_is_chosen()
        => await Host.Events
            .Stream<UserOnboardingState, UserOnboardingId>(_userOnboardingId)
            .ShouldAppendExactly(
                _userOnboardingStreamBeforeWhen,
                DomainEvent.UserAliasChosen()
                    .WithAlias(_alias)
                    .Build());

    [Test]
    public async Task Then_user_alias_is_claimed()
        => await Host.Events
            .Stream<ClaimingUserAliasState, ClaimingUserAliasId>(new ClaimingUserAliasId(_alias))
            .ShouldAppendExactly(
                _userAliasClaimStreamBeforeWhen,
                DomainEvent.UserAliasClaimed()
                    .WithUserOnboardingId(_userOnboardingId)
                    .Build());
}
