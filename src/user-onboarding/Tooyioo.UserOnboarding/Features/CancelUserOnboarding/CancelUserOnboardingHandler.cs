using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooyioo.UserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Support;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.UserOnboarding.Features.CancelUserOnboarding;

public sealed class CancelUserOnboardingHandler
    : ICommandHandler<CancelUserOnboardingCommand, CancelUserOnboardingCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IEventWriter _eventWriter;

    public CancelUserOnboardingHandler(
        IEventReader eventReader,
        IEventWriter eventWriter)
    {
        _eventReader = eventReader;
        _eventWriter = eventWriter;
    }

    public async Task<CancelUserOnboardingCommandResult> Handle(
        CancelUserOnboardingCommand command,
        CancellationToken cancellationToken = default)
    {
        var userOnboardingId = command.UserOnboardingId;

        var userOnboarding =
            await _eventReader.LoadStateOrNew<UserOnboardingState, UserOnboardingId>(userOnboardingId, cancellationToken);

        if (userOnboarding.Events.Length == 0)
        {
            return new CancelUserOnboardingUnexpectedStateErrorResult();
        }

        if (userOnboarding.State.IsComplete)
        {
            return new CancelUserOnboardingCompletedErrorResult();
        }

        if (userOnboarding.State.IsCanceled)
        {
            return new CancelUserOnboardingCommandOkResult(userOnboardingId);
        }

        var externalIdentityClaiming =
            await _eventReader.LoadStateOrNew<ClaimingExternalIdentityState, ClaimingExternalIdentityId>(
                userOnboarding.State.ExternalId,
                cancellationToken);

        if (externalIdentityClaiming.State.UserOnboardingId != userOnboardingId)
        {
            return new CancelUserOnboardingUnexpectedStateErrorResult();
        }

        var stateChanges = new List<StateStreamChanges>
        {
            userOnboarding.ToStateStreamChanges([
                new UserOnboardingDomainEvents.V1.UserOnboardingCanceled(command.Reason)
            ]),
            externalIdentityClaiming.ToStateStreamChanges([
                new UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityReleased(userOnboardingId)
            ])
        };

        if (userOnboarding.State.Alias is { } alias)
        {
            var userAliasClaiming =
                await _eventReader.LoadStateOrNew<ClaimingUserAliasState, ClaimingUserAliasId>(
                    new ClaimingUserAliasId(alias),
                    cancellationToken);

            if (userAliasClaiming.State.UserOnboardingId != userOnboardingId)
            {
                return new CancelUserOnboardingUnexpectedStateErrorResult();
            }

            stateChanges.Add(userAliasClaiming.ToStateStreamChanges([
                new UserAliasClaimingDomainEvents.V1.UserAliasReleased(userOnboardingId)
            ]));
        }

        try
        {
            _ = await _eventWriter.StoreStateChanges(stateChanges, cancellationToken);

            return new CancelUserOnboardingCommandOkResult(userOnboardingId);
        }
        catch (OptimisticConcurrencyException)
        {
            return new CancelUserOnboardingConcurrencyConflictError();
        }
    }
}

public sealed record CancelUserOnboardingCommand(UserOnboardingId UserOnboardingId, string Reason)
    : ICommand<CancelUserOnboardingCommandResult>;

[Result<CancelUserOnboardingCommandOkResult, CancelUserOnboardingCommandErrorResult>]
public partial class CancelUserOnboardingCommandResult;

public sealed record CancelUserOnboardingCommandOkResult(UserOnboardingId UserOnboardingId);

[Union<CancelUserOnboardingConcurrencyConflictError, CancelUserOnboardingCompletedErrorResult, CancelUserOnboardingUnexpectedStateErrorResult>]
public partial class CancelUserOnboardingCommandErrorResult;
public sealed record CancelUserOnboardingConcurrencyConflictError;
public sealed record CancelUserOnboardingCompletedErrorResult;
public sealed record CancelUserOnboardingUnexpectedStateErrorResult;
