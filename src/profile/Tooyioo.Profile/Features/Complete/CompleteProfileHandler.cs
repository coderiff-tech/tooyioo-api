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
    : ICommandHandler<CompleteIdentityProfileCommand, CompleteProfileCommandResult>
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
    
    public async Task<CompleteProfileCommandResult> Handle(
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
            return new CompleteProfileAliasInUseError(profileId);
        }
        
        var profileAggregate =
            await _eventReader.LoadAggregateOrNew<ProfileAggregate, ProfileState, ProfileId>(
                profileId, cancellationToken);
        
        profileAggregate.CompleteProfile(
            command.Alias);

        try
        {
            await _multiAppendEventWriter.StoreAggregatesAtomically<
                ProfileAggregate, ProfileState, ProfileId,
                UniqueAliasAggregate, UniqueAliasState, UniqueAliasId>(
                profileAggregate, uniqueAliasAggregate, cancellationToken);
            
            return new CompleteProfileCommandOkResult(profileId);
        }
        catch (OptimisticConcurrencyException)
        {
            return new CompleteProfileConcurrencyError();
        }
    }
}

public sealed record CompleteIdentityProfileCommand(ProfileId ProfileId, string Alias)
    : ICommand<CompleteProfileCommandResult>;
    
[Result<CompleteProfileCommandOkResult, CompleteProfileCommandErrorResult>]
public partial class CompleteProfileCommandResult;

public sealed record CompleteProfileCommandOkResult(ProfileId Id);

[Union<CompleteProfileAliasInUseError, CompleteProfileConcurrencyError>]
public partial class CompleteProfileCommandErrorResult;

public sealed record CompleteProfileAliasInUseError(ProfileId ProfileId);
public sealed record CompleteProfileConcurrencyError;