using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooiyoo.Identity.Domain;
using Tooiyoo.Identity.Domain.UniqueAlias;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Features.CompleteProfile;

public sealed class CompleteIdentityProfileHandler
    : ICommandHandler<CompleteIdentityProfileCommand, CompleteIdentityProfileCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IMultiAppendEventWriter _multiAppendEventWriter;

    public CompleteIdentityProfileHandler(
        IEventReader eventReader,
        IMultiAppendEventWriter multiAppendEventWriter)
    {
        _eventReader = eventReader;
        _multiAppendEventWriter = multiAppendEventWriter;
    }
    
    public async Task<CompleteIdentityProfileCommandResult> Handle(
        CompleteIdentityProfileCommand command, 
        CancellationToken cancellationToken = default)
    {
        var identityId = command.IdentityId;
        var uniqueAliasAggregate =
            await _eventReader.LoadAggregateOrNew<UniqueAliasAggregate, UniqueAliasState, UniqueAliasId>(
                command.Alias, cancellationToken);

        var claimUniqueAliasResult = uniqueAliasAggregate.Claim(identityId);
        var hasUniqueAliasError = claimUniqueAliasResult.IsErr(out var uniqueAliasAlreadyClaimedError);
        
        var isClaimedByOther =
            hasUniqueAliasError
            && uniqueAliasAlreadyClaimedError is not null
            && uniqueAliasAlreadyClaimedError.IdentityAliasClaimedBy == identityId;
        
        if (isClaimedByOther)
        {
            return new CompleteIdentityAliasInUseError(identityId);
        }
        
        var identityAggregate =
            await _eventReader.LoadAggregateOrNew<IdentityAggregate, IdentityState, IdentityId>(
                identityId, cancellationToken);
        
        identityAggregate.CompleteProfile(
            command.Alias, 
            command.PhoneNumber);

        try
        {
            await _multiAppendEventWriter.StoreAggregatesAtomically<
                IdentityAggregate, IdentityState, IdentityId,
                UniqueAliasAggregate, UniqueAliasState, UniqueAliasId>(
                identityAggregate, uniqueAliasAggregate, cancellationToken);
            
            return new CompleteIdentityProfileCommandOkResult(identityId);
        }
        catch (OptimisticConcurrencyException)
        {
            return new CompleteIdentityConcurrencyError();
        }
    }
}

public sealed record CompleteIdentityProfileCommand(IdentityId IdentityId, string Alias, string PhoneNumber)
    : ICommand<CompleteIdentityProfileCommandResult>;
    
[Result<CompleteIdentityProfileCommandOkResult, CompleteIdentityProfileCommandErrorResult>]
public partial class CompleteIdentityProfileCommandResult;

public sealed record CompleteIdentityProfileCommandOkResult(IdentityId Id);

[Union<CompleteIdentityAliasInUseError, CompleteIdentityConcurrencyError>]
public partial class CompleteIdentityProfileCommandErrorResult;

public sealed record CompleteIdentityAliasInUseError(IdentityId IdentityId);
public sealed record CompleteIdentityConcurrencyError;