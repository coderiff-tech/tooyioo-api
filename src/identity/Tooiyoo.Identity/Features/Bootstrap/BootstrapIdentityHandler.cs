using Eventuous;
using Funzo;
using Slicent.Application.Commands;
using Slicent.EventStore;
using Tooiyoo.Identity.Domain;
using Tooiyoo.Identity.Domain.UniqueExternalIdentity;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Features.Bootstrap;

public sealed class BootstrapIdentityHandler
    : ICommandHandler<BootstrapIdentityCommand, BootstrapIdentityCommandResult>
{
    private readonly IEventReader _eventReader;
    private readonly IMultiAppendEventWriter _multiAppendEventWriter;

    public BootstrapIdentityHandler(
        IEventReader eventReader,
        IMultiAppendEventWriter multiAppendEventWriter)
    {
        _eventReader = eventReader;
        _multiAppendEventWriter = multiAppendEventWriter;
    }

    public async Task<BootstrapIdentityCommandResult> Handle(
        BootstrapIdentityCommand command, 
        CancellationToken cancellationToken = default)
    {
        var identityId = command.IdentityId;
        var uniqueExternalIdentityAggregate =
            await _eventReader.LoadAggregateOrNew<UniqueExternalIdentityAggregate, UniqueExternalIdentityState, UniqueExternalIdentityId>(
                command.ExternalId, cancellationToken);

        var claimUniqueExternalIdentityResult = uniqueExternalIdentityAggregate.Claim(identityId);
        var hasUniqueExternalIdentityError = claimUniqueExternalIdentityResult.IsErr(out var externalIdentityAlreadyClaimedError);
        if (hasUniqueExternalIdentityError)
        {
            // Idempotency guaranteed if external identity is already claimed
            return new BootstrapIdentityCommandOkResult(
                externalIdentityAlreadyClaimedError!.ExternalIdentityClaimedBy, 
                command.ExternalId, 
                command.ExternalIdentityProvider);
        }
        
        var identityAggregate =
            await _eventReader.LoadAggregateOrNew<IdentityAggregate, IdentityState, IdentityId>(
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
                IdentityAggregate, IdentityState, IdentityId,
                UniqueExternalIdentityAggregate, UniqueExternalIdentityState, UniqueExternalIdentityId>(
                identityAggregate, uniqueExternalIdentityAggregate, cancellationToken);
            
            return new BootstrapIdentityCommandOkResult(
                identityId,
                command.ExternalId,
                command.ExternalIdentityProvider);
        }
        catch (OptimisticConcurrencyException)
        {
            return new BootstrapIdentityConcurrencyErrorResult();
        }
    }
}

public sealed record BootstrapIdentityCommand(
    IdentityId IdentityId,
    string Name, 
    string LastName, 
    string Email, 
    bool IsEmailConfirmed,
    string ExternalId,
    string ExternalIdentityProvider)
    : ICommand<BootstrapIdentityCommandResult>;

[Result<BootstrapIdentityCommandOkResult, BootstrapIdentityConcurrencyErrorResult>]
public partial class BootstrapIdentityCommandResult;

public sealed record BootstrapIdentityCommandOkResult(IdentityId IdentityId, string ExternalId, string ExternalIdentityProvider);

public sealed record BootstrapIdentityConcurrencyErrorResult;