using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooyioo.UserOnboarding.Contracts;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.UserOnboarding.Features.Initiate;

public sealed class InitiateOnboardingHandler
    : ICommandHandler<InitiateOnboardingCommand, InitiateOnboardingCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IEventWriter _eventWriter;

    public InitiateOnboardingHandler(
        IEventReader eventReader,
        IEventWriter eventWriter)
    {
        _eventReader = eventReader;
        _eventWriter = eventWriter;
    }

    public async Task<InitiateOnboardingCommandResult> Handle(
        InitiateOnboardingCommand command, 
        CancellationToken cancellationToken = default)
    {
        var externalId = command.ExternalId;
        var externalIdentityProvider = command.ExternalIdentityProvider;

        var onboardingUniqueExternalIdentity =
            await _eventReader.LoadStateOrNew<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
                externalId,
                cancellationToken);

        if (onboardingUniqueExternalIdentity.State.OnboardingId is not null)
        {
            return new InitiateUserOnboardingOkResult(
                onboardingUniqueExternalIdentity.State.OnboardingId,
                externalId,
                externalIdentityProvider);
        }
        
        var onboardingId = command.UserOnboardingId;
        
        var onboardingEvents = new List<object>
        {
            new UserOnboardingDomainEvents.V1.Initiated(command.Name, command.LastName, command.Email)
        };
        
        if (!string.IsNullOrWhiteSpace(command.ExternalId))
        {
            onboardingEvents.Add(
                new UserOnboardingDomainEvents.V1.ExternalIdAssociated(
                    command.ExternalId,
                    command.ExternalIdentityProvider));
        }

        if (command.IsEmailConfirmed)
        {
            onboardingEvents.Add(new UserOnboardingDomainEvents.V1.EmailVerified());
        }
        
        var onboarding =
            await _eventReader.LoadStateOrNew<UserOnboardingState, UserOnboardingId>(
                onboardingId,
                cancellationToken);

        var claimingExternalIdentityEvents =
            new object[] { new ClaimingExternalIdentityDomainEvents.V1.Claimed(onboardingId) };
        
        try
        {
            _ = await _eventWriter.StoreStateChanges(
                [
                    onboarding.ToStateStreamChanges(onboardingEvents),
                    onboardingUniqueExternalIdentity.ToStateStreamChanges(claimingExternalIdentityEvents)
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

public sealed record InitiateOnboardingCommand(
    UserOnboardingId UserOnboardingId,
    string Name, 
    string LastName, 
    string Email, 
    bool IsEmailConfirmed,
    string ExternalId,
    string ExternalIdentityProvider)
    : ICommand<InitiateOnboardingCommandResult>;

[Result<InitiateUserOnboardingOkResult, InitiateUserOnboardingConcurrencyErrorResult>]
public partial class InitiateOnboardingCommandResult;

public sealed record InitiateUserOnboardingOkResult(UserOnboardingId UserOnboardingId, string ExternalId, string ExternalIdProvider);

public sealed record InitiateUserOnboardingConcurrencyErrorResult;