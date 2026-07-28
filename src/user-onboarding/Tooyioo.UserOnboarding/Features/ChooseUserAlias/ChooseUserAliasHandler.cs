using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooyioo.UserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Support;

// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserAlias;

public sealed class ChooseUserAliasHandler
    : ICommandHandler<ChooseUserAliasCommand, ChooseUserAliasCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IEventWriter _eventWriter;

    public ChooseUserAliasHandler(
        IEventReader eventReader,
        IEventWriter eventWriter)
    {
        _eventReader = eventReader;
        _eventWriter = eventWriter;
    }
    
    public async Task<ChooseUserAliasCommandResult> Handle(
        ChooseUserAliasCommand command, 
        CancellationToken cancellationToken = default)
    {
        var userOnboardingId = command.UserOnboardingId;
        var alias = command.Alias;
        
        var claimingUserAlias =
            await _eventReader.LoadStateOrNew<ClaimingUserAliasState, ClaimingUserAliasId>(alias, cancellationToken);

        if (claimingUserAlias.State.UserOnboardingId is { } claimingUserOnboardingId
            && claimingUserOnboardingId != userOnboardingId)
        {
            return new ChooseUserAliasAlreadyInUseError(claimingUserOnboardingId);
        }
        
        var userOnboarding =
            await _eventReader.LoadStateOrNew<UserOnboardingState, UserOnboardingId>(userOnboardingId, cancellationToken);
        
        if (userOnboarding.Events.Length == 0)
        {
            return new ChooseUserAliasUnexpectedStateErrorResult();
        }

        if (userOnboarding.State.IsCanceled)
        {
            return new ChooseUserAliasCanceledErrorResult();
        }

        if (userOnboarding.State.Alias is not null)
        {
            if (userOnboarding.State.Alias == alias)
            {
                return new ChooseUserAliasCommandOkResult(userOnboardingId);
            }
            
            return new ChooseUserAliasDifferentAlreadyChosenError(userOnboarding.State.Alias);
        }
        
        var userOnboardingEvents =
            new object[] { new UserOnboardingDomainEvents.V1.UserAliasChosen(alias) };
        
        var claimingUserAliasEvents =
            new object[] { new UserAliasClaimingDomainEvents.V1.UserAliasClaimed(userOnboardingId) };
        
        try
        {
            _ = await _eventWriter.StoreStateChanges(
                [
                    userOnboarding.ToStateStreamChanges(userOnboardingEvents),
                    claimingUserAlias.ToStateStreamChanges(claimingUserAliasEvents)
                ],
                cancellationToken);

            return new ChooseUserAliasCommandOkResult(userOnboardingId);
        }
        catch (OptimisticConcurrencyException)
        {
            return new ChooseUserAliasConcurrencyConflictError();
        }
    }
}

public sealed record ChooseUserAliasCommand(UserOnboardingId UserOnboardingId, string Alias)
    : ICommand<ChooseUserAliasCommandResult>;
    
[Result<ChooseUserAliasCommandOkResult, ChooseUserAliasCommandErrorResult>]
public partial class ChooseUserAliasCommandResult;

public sealed record ChooseUserAliasCommandOkResult(UserOnboardingId UserOnboardingId);

[Union<ChooseUserAliasAlreadyInUseError, ChooseUserAliasDifferentAlreadyChosenError, ChooseUserAliasConcurrencyConflictError, ChooseUserAliasUnexpectedStateErrorResult, ChooseUserAliasCanceledErrorResult>]
public partial class ChooseUserAliasCommandErrorResult;
public sealed record ChooseUserAliasAlreadyInUseError(UserOnboardingId UserOnboardingId);
public sealed record ChooseUserAliasDifferentAlreadyChosenError(string ExistingAlias);
public sealed record ChooseUserAliasConcurrencyConflictError;
public sealed record ChooseUserAliasUnexpectedStateErrorResult;
public sealed record ChooseUserAliasCanceledErrorResult;
