using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooyioo.Profile.Domain;
using Tooyioo.Profile.Domain.UniqueExternalIdentity;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.Bootstrap;

public sealed class BootstrapProfileHandler
    : ICommandHandler<BootstrapProfileCommand, BootstrapProfileCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IMultiAppendEventWriter _multiAppendEventWriter;

    public BootstrapProfileHandler(
        IEventReader eventReader,
        IMultiAppendEventWriter multiAppendEventWriter)
    {
        _eventReader = eventReader;
        _multiAppendEventWriter = multiAppendEventWriter;
    }

    public async Task<BootstrapProfileCommandResult> Handle(
        BootstrapProfileCommand command, 
        CancellationToken cancellationToken = default)
    {
        var identityId = command.ProfileId;
        var uniqueExternalIdentityAggregate =
            await _eventReader.LoadAggregateOrNew<UniqueExternalIdentityAggregate, UniqueExternalIdentityState, UniqueExternalIdentityId>(
                command.ExternalId, cancellationToken);

        var claimUniqueExternalIdentityResult = uniqueExternalIdentityAggregate.Claim(identityId);
        var hasUniqueExternalIdentityError = claimUniqueExternalIdentityResult.IsErr(out var externalIdentityAlreadyClaimedError);
        if (hasUniqueExternalIdentityError)
        {
            // Idempotency guaranteed if external identity is already claimed
            return new BootstrapProfileCommandOkResult(
                externalIdentityAlreadyClaimedError!.ExternalProfileClaimedBy, 
                command.ExternalId, 
                command.ExternalIdentityProvider);
        }
        
        var identityAggregate =
            await _eventReader.LoadAggregateOrNew<ProfileAggregate, ProfileState, ProfileId>(
                identityId, cancellationToken);
        
        identityAggregate.Bootstrap(
            command.Name, 
            command.LastName, 
            command.Email, 
            command.IsEmailConfirmed, 
            command.ExternalId, 
            command.ExternalIdentityProvider);

        try
        {
            await _multiAppendEventWriter.StoreAggregatesAtomically<
                ProfileAggregate, ProfileState, ProfileId,
                UniqueExternalIdentityAggregate, UniqueExternalIdentityState, UniqueExternalIdentityId>(
                identityAggregate, uniqueExternalIdentityAggregate, cancellationToken);
            
            return new BootstrapProfileCommandOkResult(
                identityId,
                command.ExternalId,
                command.ExternalIdentityProvider);
        }
        catch (OptimisticConcurrencyException)
        {
            return new BootstrapProfileConcurrencyErrorResult();
        }
    }
}

public sealed record BootstrapProfileCommand(
    ProfileId ProfileId,
    string Name, 
    string LastName, 
    string Email, 
    bool IsEmailConfirmed,
    string ExternalId,
    string ExternalIdentityProvider)
    : ICommand<BootstrapProfileCommandResult>;

[Result<BootstrapProfileCommandOkResult, BootstrapProfileConcurrencyErrorResult>]
public partial class BootstrapProfileCommandResult;

public sealed record BootstrapProfileCommandOkResult(ProfileId ProfileId, string ExternalId, string ExternalIdProvider);

public sealed record BootstrapProfileConcurrencyErrorResult;