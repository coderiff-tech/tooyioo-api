using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooyioo.UserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding;

public sealed class InitiateUserOnboardingHandler
    : ICommandHandler<InitiateUserOnboardingCommand, InitiateUserOnboardingCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IEventWriter _eventWriter;

    public InitiateUserOnboardingHandler(
        IEventReader eventReader,
        IEventWriter eventWriter)
    {
        _eventReader = eventReader;
        _eventWriter = eventWriter;
    }

    public async Task<InitiateUserOnboardingCommandResult> Handle(
        InitiateUserOnboardingCommand command, 
        CancellationToken cancellationToken = default)
    {
        var externalId = command.ExternalId;
        var externalIdentityProvider = command.ExternalIdentityProvider;

        var claimingExternalIdentity =
            await _eventReader.LoadStateOrNew<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
                externalId,
                cancellationToken);

        if (claimingExternalIdentity.State.UserOnboardingId is not null)
        {
            // Idempotent: trusted ExternalId means this is the same principal retrying.
            return new InitiateUserOnboardingOkResult(
                claimingExternalIdentity.State.UserOnboardingId,
                externalId,
                externalIdentityProvider);
        }
        
        var onboardingId = command.UserOnboardingId;
        
        var userOnboarding =
            await _eventReader.LoadStateOrNew<UserOnboardingState, UserOnboardingId>(onboardingId, cancellationToken);
        if (userOnboarding.Events.Length != 0)
        {
            return new InitiateUserOnboardingUnexpectedStateErrorResult();
        }
        
        var userOnboardingEvents = new List<object>
        {
            new UserOnboardingDomainEvents.V1.UserOnboardingInitiated(command.Name, command.LastName, command.Email),
            new UserOnboardingDomainEvents.V1.UserExternalIdAssociated(externalId, externalIdentityProvider)
        };

        if (command.IsEmailConfirmed)
        {
            userOnboardingEvents.Add(new UserOnboardingDomainEvents.V1.UserEmailVerified());
        }

        var claimingExternalIdentityEvents =
            new object[] { new UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed(onboardingId) };
        
        try
        {
            _ = await _eventWriter.StoreStateChanges(
                [
                    userOnboarding.ToStateStreamChanges(userOnboardingEvents),
                    claimingExternalIdentity.ToStateStreamChanges(claimingExternalIdentityEvents)
                ],
                cancellationToken);

            return new InitiateUserOnboardingOkResult(
                onboardingId,
                externalId,
                externalIdentityProvider);
        }
        catch (OptimisticConcurrencyException)
        {
            return new InitiateUserOnboardingConcurrencyErrorResult();
        }
    }
}

public sealed record InitiateUserOnboardingCommand(
    UserOnboardingId UserOnboardingId,
    string Name, 
    string LastName, 
    string Email, 
    bool IsEmailConfirmed,
    string ExternalId,
    string ExternalIdentityProvider)
    : ICommand<InitiateUserOnboardingCommandResult>;

[Result<InitiateUserOnboardingOkResult, ChooseUserAliasCommandErrorResult>]
public partial class InitiateUserOnboardingCommandResult;

public sealed record InitiateUserOnboardingOkResult(UserOnboardingId UserOnboardingId, string ExternalId, string ExternalIdProvider);

[Union<InitiateUserOnboardingConcurrencyErrorResult, InitiateUserOnboardingUnexpectedStateErrorResult>]
public partial class ChooseUserAliasCommandErrorResult;
public sealed record InitiateUserOnboardingConcurrencyErrorResult;
public sealed record InitiateUserOnboardingUnexpectedStateErrorResult;

