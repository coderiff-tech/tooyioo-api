using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooyioo.Profile.Domain;
using Tooyioo.Profile.Domain.UniqueAlias;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.Complete;

public sealed class CompleteProfileHandler
    : ICommandHandler<CompleteIdentityProfileCommand, CompleteIdentityProfileCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IMultiAppendEventWriter _multiAppendEventWriter;

    public CompleteProfileHandler(
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
        var profileId = command.ProfileId;
        var uniqueAliasAggregate =
            await _eventReader.LoadAggregateOrNew<UniqueAliasAggregate, UniqueAliasState, UniqueAliasId>(
                command.Alias, cancellationToken);

        var claimUniqueAliasResult = uniqueAliasAggregate.Claim(profileId);
        var hasUniqueAliasError = claimUniqueAliasResult.IsErr(out var uniqueAliasAlreadyClaimedError);
        
        var isClaimedByOther =
            hasUniqueAliasError
            && uniqueAliasAlreadyClaimedError is not null
            && uniqueAliasAlreadyClaimedError.ProfileAliasClaimedBy == profileId;
        
        if (isClaimedByOther)
        {
            return new CompleteIdentityAliasInUseError(profileId);
        }
        
        var identityAggregate =
            await _eventReader.LoadAggregateOrNew<ProfileAggregate, ProfileState, ProfileId>(
                profileId, cancellationToken);
        
        identityAggregate.CompleteProfile(
            command.Alias);

        try
        {
            await _multiAppendEventWriter.StoreAggregatesAtomically<
                ProfileAggregate, ProfileState, ProfileId,
                UniqueAliasAggregate, UniqueAliasState, UniqueAliasId>(
                identityAggregate, uniqueAliasAggregate, cancellationToken);
            
            return new CompleteIdentityProfileCommandOkResult(profileId);
        }
        catch (OptimisticConcurrencyException)
        {
            return new CompleteIdentityConcurrencyError();
        }
    }
}

public sealed record CompleteIdentityProfileCommand(ProfileId ProfileId, string Alias)
    : ICommand<CompleteIdentityProfileCommandResult>;
    
[Result<CompleteIdentityProfileCommandOkResult, CompleteIdentityProfileCommandErrorResult>]
public partial class CompleteIdentityProfileCommandResult;

public sealed record CompleteIdentityProfileCommandOkResult(ProfileId Id);

[Union<CompleteIdentityAliasInUseError, CompleteIdentityConcurrencyError>]
public partial class CompleteIdentityProfileCommandErrorResult;

public sealed record CompleteIdentityAliasInUseError(ProfileId ProfileId);
public sealed record CompleteIdentityConcurrencyError;